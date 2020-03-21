namespace WorldMap.Effects
{
    public enum MapEffectState
    {
        Unavailable,
        Normal,
        Highlighted,
        HasCameraFocus // If we're zoomed in on the View (it's the "Current" View)
    }
}