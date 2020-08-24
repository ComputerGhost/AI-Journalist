using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace AI_Journalist.Contexts
{
    class Vision
    {
        ComputerVisionClient Service;

        public Vision(Settings.ContextsNode.VisionNode settings)
        {
            Service = new ComputerVisionClient(
                new ApiKeyServiceClientCredentials(settings.ApiKey),
                new DelegatingHandler[] { });
            Service.Endpoint = settings.Endpoint;
        }

        public void AddContext(Context context)
        {
            foreach (var media in context.Source.Medias) {

                var features = new VisualFeatureTypes?[] { VisualFeatureTypes.Description };
                var result = Service.AnalyzeImageAsync(media.DisplayUrl, features).Result;

                var description = new Context.MediaDescription();
                if (result.Description.Captions.Count > 0) {
                    var topCaption = result.Description.Captions[0];
                    description.Description = topCaption.Text;
                    description.Confidence = topCaption.Confidence;
                    description.AssociatedMedia = media;
                }
                context.MediaDescriptions.Add(description);
            }
        }
    }
}
