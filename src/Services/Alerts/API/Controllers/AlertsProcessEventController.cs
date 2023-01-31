﻿using Dapr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Commands;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Events;
using SolTechnology.Avro;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.EventControllers
{
    [ApiController]
    [Route("[controller]")]
    public class AlertsProcessEventController : ControllerBase
    {
        private readonly ILogger<AlertsProcessEventController> _logger;
        private readonly IMediator _mediator;


        public AlertsProcessEventController(ILogger<AlertsProcessEventController> logger, IMediator mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [Topic("pubsub", "newAlert")]
        [HttpPost]
        public async Task PostAlert(byte[] alertBytes)
        {
            var detection = AvroConvert.Deserialize<DetectedObjectAlert>(alertBytes);

            var nFrame = await _mediator.Send(new PaintBoundingBoxesCommand() 
            { 
                BoundingBoxPoints = detection.BoundingBoxes, 
                OriginalImageBase64 = detection.Frame, 
            });

            await _mediator.Send(new PersistAlertCommand()
            {
                Information = detection.Information,
                CaptureTime = detection.EveryTime,
                Type = detection.Type,
                Frame = nFrame,
                Accuracy = detection.Accuracy
            });
            _logger.LogInformation("Stored generic alert");
        }
    }
}