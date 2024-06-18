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

        public void OnPointerClick(PointerEventData eventData)
        {
            if (keyControlled)
                InvokeCompleteActionEvent();
        }
    }
}
