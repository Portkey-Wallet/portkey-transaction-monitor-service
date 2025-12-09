using System;
using System.IO;
using System.Linq;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Extensions.DependencyInjection;
using Microsoft.AspNetCore.RateLimiting;
using OpenIddict.Validation.AspNetCore;
using Transaction.Monitor.MongoDB;
using Transaction.Monitor.MultiTenancy;
using Microsoft.OpenApi.Models;
using Volo.Abp;
using Volo.Abp.Account.Web;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite.Bundling;
using Transaction.Monitor.AddressConfigs;
using Transaction.Monitor.Common;
using Transaction.Monitor.GuardKeys;
using Transaction.Monitor.Options;
using Transaction.Monitor.Scans;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Swashbuckle;
using Volo.Abp.Studio.Client.AspNetCore;
using Volo.Abp.Security.Claims;

namespace Transaction.Monitor;

[DependsOn(
    typeof(MonitorHttpApiModule),
    typeof(AbpStudioClientAspNetCoreModule),
    typeof(AbpAspNetCoreMvcUiLeptonXLiteThemeModule),
    typeof(AbpAutofacModule),
    typeof(AbpAspNetCoreMultiTenancyModule),
    typeof(MonitorApplicationModule),
    typeof(MonitorMongoDbModule),
    typeof(AbpAccountWebOpenIddictModule),
    typeof(AbpSwashbuckleModule),
    typeof(AbpAspNetCoreSerilogModule)
)]
public class MonitorHttpApiHostModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        Configure<ChainOptions>(configuration.GetSection("Chains"));

        var hostingEnvironment = context.Services.GetHostingEnvironment();
        context.Services.AddScoped<IGuardKeyAppService, GuardKeyAppService>();
        context.Services.AddScoped<IAddressConfigAppService, AddressConfigAppService>();
        context.Services.AddHostedService<RetryCallbackService>();
        context.Services.AddScoped<CallbackService>();
        context.Services.AddHostedService<ScanTransactionService>();
        context.Services.AddRateLimiter(_ => _
            .AddSlidingWindowLimiter(policyName: RateLimitConstant.SendTxPolicy, options =>
            {
                options.PermitLimit = RateLimitConstant.SendTxPermitLimit;
                options.Window = TimeSpan.FromSeconds(RateLimitConstant.SendTxWindow);
                options.SegmentsPerWindow = RateLimitConstant.SendTxSegmentsPerWindow;
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                options.QueueLimit = RateLimitConstant.SendTxQueueLimit;
            }));

        PreConfigure<OpenIddictBuilder>(builder =>
        {
            builder.AddValidation(options =>
            {
                options.AddAudiences("Monitor");
                options.UseLocalServer();
                options.UseAspNetCore();
            });
        });
        context.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        if (!configuration.GetValue<bool>("App:DisablePII"))
        {
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
        }

        ConfigureAuthentication(context);
        ConfigureUrls(configuration);
        ConfigureBundles();
        ConfigureConventionalControllers();
        ConfigureSwagger(context, configuration);
        ConfigureVirtualFileSystem(context);
    }

    private void ConfigureAuthentication(ServiceConfigurationContext context)
    {
        context.Services.ForwardIdentityAuthenticationForBearer(OpenIddictValidationAspNetCoreDefaults
            .AuthenticationScheme);
        context.Services.Configure<AbpClaimsPrincipalFactoryOptions>(options => { options.IsDynamicClaimsEnabled = true; });
    }

    private void ConfigureUrls(IConfiguration configuration)
    {
        Configure<AppUrlOptions>(options =>
        {
            options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
            options.RedirectAllowedUrls.AddRange(configuration["App:RedirectAllowedUrls"]?.Split(',') ??
                                                 Array.Empty<string>());
        });
    }

    private void ConfigureBundles()
    {
        Configure<AbpBundlingOptions>(options =>
        {
            options.StyleBundles.Configure(
                LeptonXLiteThemeBundles.Styles.Global,
                bundle => { bundle.AddFiles("/global-styles.css"); }
            );
        });
    }


    private void ConfigureVirtualFileSystem(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<MonitorDomainSharedModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}Transaction.Monitor.Domain.Shared"));
                options.FileSets.ReplaceEmbeddedByPhysical<MonitorDomainModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}Transaction.Monitor.Domain"));
                options.FileSets.ReplaceEmbeddedByPhysical<MonitorApplicationContractsModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}Transaction.Monitor.Application.Contracts"));
                options.FileSets.ReplaceEmbeddedByPhysical<MonitorApplicationModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}Transaction.Monitor.Application"));
            });
        }
    }

    private void ConfigureConventionalControllers()
    {
        Configure<AbpAspNetCoreMvcOptions>(options => { options.ConventionalControllers.Create(typeof(MonitorApplicationModule).Assembly); });
    }

    private static void ConfigureSwagger(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Transaction Monitor v1", Version = "v1" });
            c.TagActionsBy(api => new[] { api.GroupName });
            c.OperationFilter<RemoveAuthorizeFilter>();
            c.DocInclusionPredicate((docName, apiDesc) => apiDesc.ActionDescriptor.DisplayName.Contains("Transaction.Monitor") &&
                                                          apiDesc.ActionDescriptor.DisplayName.Contains("Controller"));
        });
    }

    private void ConfigureCors(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .WithOrigins(
                        configuration["App:CorsOrigins"]?
                            .Split(",", StringSplitOptions.RemoveEmptyEntries)
                            .Select(o => o.Trim().RemovePostFix("/"))
                            .ToArray() ?? Array.Empty<string>()
                    )
                    .WithAbpExposedHeaders()
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseAbpRequestLocalization();

        if (!env.IsDevelopment())
        {
            app.UseErrorPage();
        }

        app.UseStaticFiles();
        app.UseAbpStudioLink();
        app.UseRouting();
        app.UseRateLimiter();
        app.UseAbpSecurityHeaders();
        app.UseCors("AllowAll");
        app.UseAuthentication();
        app.UseAbpOpenIddictValidation();

        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }

        app.UseUnitOfWork();
        app.UseDynamicClaims();

        app.UseSwagger();
        app.UseAbpSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "Monitor API"); });
        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseConfiguredEndpoints();
    }
}