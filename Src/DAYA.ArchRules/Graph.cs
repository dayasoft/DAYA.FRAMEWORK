using System.Collections.Generic;

namespace DAYA.ArchRules
{
    internal class Graph
    {
        private int V;
        private List<int>[] adj;

        public Graph(int v)
        {
            V = v;
            adj = new List<int>[v];
            for (int i = 0; i < v; ++i)
            {
                adj[i] = new List<int>();
            }
        }

        public void AddEdge(int v, int w)
        {
            adj[v].Add(w);
        }

        public bool IsCyclic()
        {
            bool[] visited = new bool[V];
            bool[] recStack = new bool[V];

            for (int i = 0; i < V; i++)
            {
                if (Dfs(i, visited, recStack))
                {
                    return true;
                }
            }

            return false;
        }

        private bool Dfs(int i, bool[] visited, bool[] recursiveStack)
        {
            if (recursiveStack[i])
            {
                return true;
            }

            if (visited[i])
            {
                return false;
            }

            visited[i] = true;

            recursiveStack[i] = true;
            var children = adj[i];

            foreach (int child in children)
            {
                if (Dfs(child, visited, recursiveStack))
                {
                    return true;
                }
            }

            recursiveStack[i] = false;

            return false;
        }
    }
}
