namespace GalaxyMap.Nodes
{
    public interface ILinearMapNodeManager : INodeManager
    {
        IMapNode FirstNode { get; }
        IMapNode LastNode { get; }
        
        bool SelectNext();
        bool SelectPrev();
    }
}