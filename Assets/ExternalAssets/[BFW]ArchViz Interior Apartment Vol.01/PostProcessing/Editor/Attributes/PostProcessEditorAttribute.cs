using System;

namespace UnityEngine.Rendering.PostProcessing
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class PostProcessEditorAttribute : Attribute
    {
        public readonly Type settingsType;

        public PostProcessEditorAttribute(Type settingsType)
        {
            this.settingsType = settingsType;
        }
    }
}