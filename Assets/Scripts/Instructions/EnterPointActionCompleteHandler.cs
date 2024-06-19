using UnityEngine.EventSystems;

namespace Instructions
{
    public class EnterPointActionCompleteHandler : ActionCompleteHandler, IPointerEnterHandler, IPointerClickHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!keyControlled)
                InvokeCompleteActionEvent();
            
        }

        private void Start()
        {
            IsKeyPressed = true;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (keyControlled)
                InvokeCompleteActionEvent();
        }
    }
}
