namespace Catalog.Core.Models;

/// <summary>
///     Options to configure Jaeger tracing
/// </summary>
public class JaegerOptions
{
    /// <summary>
    ///     Name of service to be displayed
    /// </summary>
    public string ServiceName { get; set; }

    /// <summary>
    ///     Jaeger host
    /// </summary>
    public string Host { get; set; }

    /// <summary>
    ///     Jaeger port
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    ///     Span sampling rate
    /// </summary>
    public double SamplingRate { get; set; }

    /// <summary>
    ///     Lower bound for guaranteed sampler
    /// </summary>
    public double LowerBound { get; set; }
}