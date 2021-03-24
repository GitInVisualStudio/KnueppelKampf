using KnueppelKampfBase.Utils;
using KnueppelKampfBase.Utils.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KnueppelKampfBase.Game
{
    public class WorldDelta
    {
        private int oldId;
        private int newId;
        private List<int> deleted;
        private List<GameObject> spawned;
        private List<ObjectDelta> changed;
        public int EarlierId { get => oldId; set => oldId = value; }
        public int NewerId { get => newId; set => newId = value; }
        public List<int> Deleted { get => deleted; set => deleted = value; }
        public List<GameObject> Spawned { get => spawned; set => spawned = value; }
        public List<ObjectDelta> Changed { get => changed; set => changed = value; }

        public WorldDelta(WorldState oldState, WorldState newState)
        {
            if (oldState == null)
                oldId = -1;
            else
                oldId = oldState.Id;
            newId = newState.Id;

            deleted = new List<int>();
            spawned = new List<GameObject>();
            if (oldState == null)
            {
                foreach (ObjectState os in newState.States)
                    spawned.Add(os.Obj);
                return;
            }
            foreach(ObjectState objs in oldState.States)
                if(newState.States.Find(x => objs.Id == x.Id) == null)
                    Deleted.Add(objs.Id);

            foreach (ObjectState objs in newState.States)
                if (oldState.States.Find(x => objs.Id == x.Id) == null)
                    Spawned.Add(objs.Obj);

            changed = new List<ObjectDelta>();
            for (int i = 0; i < newState.States.Count; i++)
            {
                ObjectState newOS = newState.States[i];
                ObjectState oldOS = oldState.States.Find(x => x.Id == newOS.Id);
                if (oldOS == null)
                    continue;
                ObjectDelta d = new ObjectDelta(oldOS, newOS);
                if (d.ChangedProperties.Count > 0 || d.ChangedComponents.Count > 0)
                    changed.Add(d);
            }

            deleted = new List<int>();
            foreach (ObjectState os in oldState.States)
                if (newState.States.Find(x => x.Obj.Id == os.Obj.Id) == null)
                    deleted.Add(os.Obj.Id);
        }

        public WorldDelta(byte[] bytes, int startIndex)
        {
            spawned = new List<GameObject>();
            Changed = new List<ObjectDelta>();
            deleted = new List<int>();
            int index = startIndex;
            oldId = BitConverter.ToInt32(bytes, index);
            index += sizeof(int);
            newId = BitConverter.ToInt32(bytes, index);
            index += sizeof(int);

            if (index == bytes.Length)
                return;

            // deserialize spawned objects
            int size = bytes[index++];
            GameObject[] newSpawned = new GameObject[size];
            for (int i = 0; i < newSpawned.Length; i++)
            {
                size = bytes[index++];
                GameObject obj;
                index += ObjectState.FromBytes(bytes, index, size, out obj);
                newSpawned[i] = obj;
            }
            spawned = newSpawned.ToList();

            if (index == bytes.Length)
                return;
            // deserialize changed objects
            int changedCount = bytes[index++];
            for (int i = 0; i < changedCount; i++)
            {
                size = bytes[index++];
                Changed.Add(new ObjectDelta(bytes, index));
                index += size;
            }

            if (index == bytes.Length)
                return;
            // deserialize deleted objects
            while (index < bytes.Length)
            {
                deleted.Add(BitConverter.ToInt32(bytes, index));
                index += sizeof(int);
            }
        }

        public int ToBytes(byte[] array, int startIndex)
        {
            // header
            int index = startIndex; 
            BitConverter.GetBytes(oldId).CopyTo(array, index);
            index += sizeof(int);
            BitConverter.GetBytes(newId).CopyTo(array, index);
            index += sizeof(int);

            // serialize spawned objects
            array[index++] = (byte)spawned.Count;
            foreach (GameObject go in spawned)
            {
                ObjectState os = new ObjectState(go);
                int size = os.ToBytes(array, index + 1);
                if (size > byte.MaxValue)
                    throw new SerializedSizeTooLargeException(size);
                array[index++] = (byte)size;
                index += size;
            }

            // serialize changed objects
            if (Changed == null) 
                return index - startIndex;
            array[index++] = (byte)changed.Count;
            foreach (ObjectDelta od in Changed)
            {
                int size = od.ToBytes(array, index + 1);
                array[index++] = (byte)size;
                index += size;
            }

            //serialize deleted objects
            foreach (int x in deleted)
            {
                BitConverter.GetBytes(x).CopyTo(array, index);
                index += sizeof(int);
            }

            return index - startIndex;
        }

        public static Dictionary<byte, object> GetChangedProperties(object oldObj, object newObj)
        {
            Type t = oldObj.GetType();
            if (t != newObj.GetType())
                throw new Exception("Two different object Obj types given");
            Dictionary<byte, object> result = new Dictionary<byte, object>();
            PropertyInfo[] properties = t.GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo prop = properties[i];
                if (!prop.PropertyType.IsValueType || prop.GetCustomAttribute<DontSerializeAttribute>() != null)
                    continue;
                object oldVal = prop.GetValue(oldObj);
                object newVal = prop.GetValue(newObj);
                if ((newVal == null && oldVal != null) || (newVal != null && !newVal.Equals(oldVal)))
                    result[(byte)i] = newVal;
            }
            return result;
        }
    }
}
