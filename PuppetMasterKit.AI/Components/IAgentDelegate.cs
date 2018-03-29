using System;
namespace PuppetMasterKit.AI.Components
{
    public interface IAgentDelegate
    {
        void AgentWillUpdate(Agent agent);
        void AgentDidUpdate(Agent agent);
    }
}
