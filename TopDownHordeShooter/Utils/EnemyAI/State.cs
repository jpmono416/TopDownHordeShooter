using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace TopDownHordeShooter.Utils.EnemyAI
{
    public abstract class State
    {
        public abstract void Enter(object owner);
        public abstract void Exit(object owner);
        public abstract void Execute(object owner, GameTime gameTime);
        public string Name
        {
            get;
            set;
        } 
        private List<Transition> _mTransitions = new List<Transition>();
        public List<Transition> Transitions
        {
            get { return _mTransitions; }
        }
        public void AddTransition(Transition transition)
        {
            _mTransitions.Add(transition);
        } 
    }
    
    public class Transition
    {
        public readonly State NextState;
        public readonly Func<bool> Condition;
        public Transition(State nextState, Func<bool> condition)
        {
            NextState = nextState;
            Condition = condition;
        }
    } 
}