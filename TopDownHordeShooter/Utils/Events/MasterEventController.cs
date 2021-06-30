using System.Collections.Generic;

namespace TopDownHordeShooter.Utils.Events
{
    public class BaseEventController
    {
        // Empty class extended by all the EventControllers to get them nicely on a list together
    }
    
    public class MasterEventController
    {
        public PlayerController PlayerController;
        public PickupController PickupController; 

        public MasterEventController()
        {
            PlayerController = new PlayerController();
            PickupController = new PickupController();
        }
    }
}