using System.Linq;
using UnityEngine;

namespace GalaxyMap.Utils
{
    public class MonoMouseMessageForwarder : MonoBehaviour
    {
        public enum Message
        {
            // Hover events
            OnMouseEnter,
            OnMouseExit,
            OnMouseOver,

            // Click Events
            OnMouseDown,
            OnMouseUp,
            OnMouseUpAsButton,

            // Drag Events
            OnMouseDrag
        }

        [SerializeField] private GameObject _target;
        [SerializeField] private Message[] _messages;

        public GameObject Target
        {
            get => _target;
            set => _target = value;
        }

        public Message[] Messages
        {
            get => _messages;
            set => _messages = value;
        }

        // Hover events
        private void OnMouseEnter() => ForwardIfRelevant(Message.OnMouseEnter);
        private void OnMouseExit() => ForwardIfRelevant(Message.OnMouseExit);
        private void OnMouseOver() => ForwardIfRelevant(Message.OnMouseOver);

        // Click events
        private void OnMouseDown() => ForwardIfRelevant(Message.OnMouseDown);
        private void OnMouseUp() => ForwardIfRelevant(Message.OnMouseUp);
        private void OnMouseUpAsButton() => ForwardIfRelevant(Message.OnMouseUpAsButton);

        // Drag Events
        private void OnMouseDrag() => ForwardIfRelevant(Message.OnMouseDrag);


        private void ForwardIfRelevant(Message message)
        {
            if (ShouldForward(message))
            {
                Forward(_target, message);
            }
        }

        private static void Forward(GameObject target, Message message)
        {
            if (target == null) return;
            target.SendMessage(message.ToString());
        }

        private bool ShouldForward(Message message) => _messages.Contains(message);
    }
}
