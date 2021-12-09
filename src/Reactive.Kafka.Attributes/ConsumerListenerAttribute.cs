using System;
using System.Collections.Generic;
using System.Linq;

namespace Reactive.Kafka.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ConsumerListenerAttribute : Attribute 
    { 
        private IList<string> _topics = new List<string>();

        public Type DeserializeTo = typeof(string);

        public ConsumerListenerAttribute(params string[] topics) {
            if (IsInvalid(topics)) {
                throw new Exception("Cannot create Consumer Listener without topics");
            }

            _topics = topics.ToList();
        }

        public IList<string> Topics => _topics;

        private bool IsInvalid(string[] topics) {
            return topics == null || !topics.Any() || topics.Any(x => string.IsNullOrEmpty(x));
        }
    }  
}