﻿using MediatR;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Commands;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Models;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.CommandHandlers
{
    public class PersistAlertHandler : IRequestHandler<PersistAlertCommand, Guid>
    {
        private readonly IAlertsRepository _repository;

        public PersistAlertHandler(IAlertsRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<Guid> Handle(PersistAlertCommand request, CancellationToken cancellationToken)
        {
            var id = Guid.NewGuid();
            var entity = new Alert()
            {
                Frame = request.Frame,
                AlertTriggerTimeIni = request.AlertTriggerTimeIni,
                Information = request.Information,
                Id = id,
                Type = request.Type,
                Accuracy = request.Accuracy,
            };
            entity.Source = this.SetHardwareMockInformation();
            await this._repository.Create(entity);
            return id;
        }

        private Source SetHardwareMockInformation()
        {
            var randomGenerator = new Random();
            var randomCameraNumber = randomGenerator.Next(1, 10);
            var source = new Source();
            source.Name = $"Camera {randomCameraNumber}";
            source.lat = randomGenerator.Next(1, 100);
            source.@long = randomGenerator.Next(1, 10);
            source.Type = "Camera";

            return source;
        }
    }
}
