using Microsoft.Diagnostics.Runtime;
using System.Collections.Generic;

namespace KeeTheft
{
    class ClrMDHelper
    {
        // Returns all objects being kept alive by the input object
        // Based off the code at https://github.com/Microsoft/clrmd/blob/master/Documentation/WalkingTheHeap.md
        public static List<ulong> GetReferencedObjects(ClrHeap heap, ulong obj)
        {
            List<ulong> references = new List<ulong>();
            Stack<ulong> eval = new Stack<ulong>();

            HashSet<ulong> considered = new HashSet<ulong>();

            eval.Push(obj);

            while (eval.Count > 0)
            {
                obj = eval.Pop();
                if (considered.Contains(obj))
                    continue;

                considered.Add(obj);

                // Grab the type. We will only get null here in the case of heap corruption.
                ClrType type = heap.GetObjectType(obj);
                if (type == null)
                    continue;

                references.Add(obj);

                // Now enumerate all objects that this object points to, add them to the
                // evaluation stack if we haven't seen them before.
                type.EnumerateRefsOfObjectCarefully(obj, delegate (ulong child, int offset)
                {
                    if (child != 0 && !considered.Contains(child))
                        eval.Push(child);
                });
            }

            return references;
        }
    }
}
