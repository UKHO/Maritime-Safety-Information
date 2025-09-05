using UKHO.ADDS.Mocks.Markdown;
using UKHO.ADDS.Mocks.Mime;
using UKHO.ADDS.Mocks.States;

namespace UKHO.ADDS.Mocks.Override.Mocks.fssmsi
{
    public class GetFssMsiBatchesEndpoint : ServiceEndpointMock
    {
        public override void RegisterSingleEndpoint(IEndpointMock endpoint) =>
            endpoint.MapGet("/batch", (HttpRequest request) =>
                {
                    var state = GetState(request);

                    switch (state)
                    {
                        case WellKnownState.Default:
                            try
                            {
                                var fs = GetFileSystem();
                                return Results.File(fs.OpenFile("/annualfiles.json", FileMode.Open, FileAccess.Read), MimeType.Application.Json);
                            }
                            catch (Exception)
                            {
                                return Results.NotFound("Could not find the path in the /files GET method");
                            }

                        default:
                            // Just send default responses
                            return WellKnownStateHandler.HandleWellKnownState(state);
                    }
                })
                .Produces<string>()
                .WithEndpointMetadata(endpoint, d =>
                {
                    d.Append(new MarkdownHeader("Gets Batches (MSI)", 3));
                    d.Append(new MarkdownParagraph("This is driven from a static file annualfiles.json"));
                });
    }
}
