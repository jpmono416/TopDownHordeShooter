namespace TopDownHordeShooter.Utils.Events
{
    public delegate void PickupEventHandler(object sender, PickupEventArgs args);
    
    public class PickupController : BaseEventController
    {
        public event PickupEventHandler Changed;

        protected virtual void OnChanged(PickupEventArgs args) => Changed?.Invoke(this, args);

        public void Spawn(PickupEventArgs args) => OnChanged(args); 

        public void Collected(PickupEventArgs args) => OnChanged(args); 
        
        public void ApplyEffect(PickupEventArgs args) => OnChanged(args);
    }
}