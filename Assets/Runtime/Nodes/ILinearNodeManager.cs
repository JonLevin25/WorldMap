namespace GalaxyMap.Nodes
{
    public interface ILinearNodeManager : INodeManager
    {
        IGalaxyNode FirstNode { get; }
        IGalaxyNode LastNode { get; }
        
        bool SelectNext();
        bool SelectPrev();
    }
}