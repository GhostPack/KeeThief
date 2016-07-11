using KeeTheft.KeyInfo;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace KeeTheft
{
    // Most of this code was copied directly from the the KeePass source code
    public class CompositeKeyInfo
    {
        private List<IUserKey> m_vUserKeys = new List<IUserKey>();

        public IEnumerable<IUserKey> UserKeys
        {
            get { return m_vUserKeys; }
        }

        public uint UserKeyCount
        {
            get { return (uint)m_vUserKeys.Count; }
        }

        public CompositeKeyInfo()
        {

        }

        public void AddUserKey(IUserKey pKey)
        {
            Debug.Assert(pKey != null); if (pKey == null) throw new ArgumentNullException("pKey");

            m_vUserKeys.Add(pKey);
        }

        public bool RemoveUserKey(IUserKey pKey)
        {
            Debug.Assert(pKey != null); if (pKey == null) throw new ArgumentNullException("pKey");

            Debug.Assert(m_vUserKeys.IndexOf(pKey) >= 0);
            return m_vUserKeys.Remove(pKey);
        }

        public bool ContainsType(Type tUserKeyType)
        {
            Debug.Assert(tUserKeyType != null);
            if (tUserKeyType == null) throw new ArgumentNullException("tUserKeyType");

            foreach (IUserKey pKey in m_vUserKeys)
            {
                if (pKey == null) { Debug.Assert(false); continue; }

                if (tUserKeyType.IsInstanceOfType(pKey))
                    return true;
            }

            return false;
        }
    }
}
