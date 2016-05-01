# Mini Metrics
Simple .NET client library for feeding data into Graphite. Heavily inspired by the many similar libraries in Github.

```C#
// startup code
Metrics.Start( new MetricsOptions {
    Hostname = "localhost",
    Prefix = "projectName",
    Port = 2003    
} );

// normal usage
Metric.Report("accounts.authentication.login.failure.email_not_valid", 1);
Metric.Report("accounts.authentication.login.failure.invalid_password", 1);

using(Metric.ReportTimer("http.api.controller.post.save_users.response_time")){
    // WebApi Execute Action Method
}

using(Metric.ReportTimer("service.performance.query.get_users")){
    // Win Service Execute Query
}

//  cleanup
Metrics.Stop()
```