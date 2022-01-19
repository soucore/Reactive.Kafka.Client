using Microsoft.Extensions.DependencyInjection;
using Reactive.Kafka.Validations.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Reactive.Kafka.Validations
{
    public sealed class KafkaValidators<TMessage> : IEnumerable<IKafkaMessageValidator>, IEnumerable
    {
        private readonly List<IKafkaMessageValidator<TMessage>> validators = new();
        private readonly IServiceProvider _serviceProvider;

        public KafkaValidators(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public int Count => validators.Count;

        public void Add<T>()
            where T : IKafkaMessageValidator<TMessage>
        {
            var validatorInstance = (IKafkaMessageValidator<TMessage>)ActivatorUtilities
                .CreateInstance(_serviceProvider, typeof(T));

            validators.Add(validatorInstance);
        }

        public void Add(Type type)
        {
            var interfaceType = type.GetInterface(typeof(IKafkaMessageValidator<>).Name);
            if (interfaceType is null)
                throw new ArgumentException($"{type.Name} does not implement {nameof(IKafkaMessageValidator)} interface.");

            var genericTypeArgument = interfaceType.GetGenericArguments()[0];
            if (genericTypeArgument != typeof(TMessage))
                throw new ArgumentException($"You cannot validate message of type '{genericTypeArgument.Name}'. {typeof(TMessage).Name} is expected.");

            var validatorInstance = (IKafkaMessageValidator<TMessage>)ActivatorUtilities
                .CreateInstance(_serviceProvider, type);

            validators.Add(validatorInstance);
        }

        public IEnumerator<IKafkaMessageValidator> GetEnumerator()
        {
            foreach (var validator in validators)
                yield return validator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
