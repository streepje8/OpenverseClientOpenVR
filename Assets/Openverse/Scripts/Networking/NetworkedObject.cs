//-------------------------------
//NetworkedObject
//The client side handler for a fully one way networked object (Server -> Client).
//
//Author: streep
//Creation Date: 12-04-2022
//--------------------------------
namespace Openverse.NetCode
{
    using Openverse.Core;
    using Openverse.Data;
    using RiptideNetworking;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;

    public class NetworkedObject : MonoBehaviour
    {
        public Guid myID;
        private Dictionary<string, PropertyInfo> networkedProperties = new Dictionary<string, PropertyInfo>();
        private Dictionary<string, Component> networkedComponents = new Dictionary<string, Component>();

        public static void RemoveComponent(Message message)
        {
            Guid id = Guid.Parse(message.GetString());
            OpenverseNetworkClient.NetworkedObjects.TryGetValue(id, out NetworkedObject obj);
            if (obj != null)
            {
                string compType = message.GetString();
                if (obj.GetComponent(compType) != null)
                {
                    Destroy(obj.GetComponent(compType));
                }
            }
        }

        public static void AddComponent(Message message)
        {
            Guid id = Guid.Parse(message.GetString());
            OpenverseNetworkClient.NetworkedObjects.TryGetValue(id, out NetworkedObject obj);
            if (obj != null)
            {
                int index = message.GetInt();
                if (AllowedComponents.allowedTypesList.Count > index)
                {
                    Type type = AllowedComponents.allowedTypesList[index];
                    Component c = null;
                    if (obj.gameObject.GetComponent(type) != null)
                    {
                        c = obj.gameObject.GetComponent(type);
                    }
                    else
                    {
                        c = obj.gameObject.AddComponent(type);
                    }
                    int x = 0;
                    Dictionary<string, PropertyAssignment> properties = new Dictionary<string, PropertyAssignment>();

                    //Parse the properties in the packet
                    while (x < 1000 && message.GetBool())
                    {
                        string varname = message.GetString();
                        switch (message.GetUShort())
                        {
                            case 0:
                                properties.Add(varname, new PropertyAssignment(message.GetString(), true));
                                break;
                            case 1:
                                properties.Add(varname, new PropertyAssignment(message.GetFloat(), true));
                                break;
                            case 2:
                                properties.Add(varname, new PropertyAssignment(message.GetInt(), true));
                                break;
                            case 3:
                                properties.Add(varname, new PropertyAssignment(message.GetBool(), true));
                                break;
                            case 4:
                                properties.Add(varname, new PropertyAssignment(message.GetVector2(), true));
                                break;
                            case 5:
                                properties.Add(varname, new PropertyAssignment(message.GetVector3(), true));
                                break;
                            case 6:
                                properties.Add(varname, new PropertyAssignment(message.GetQuaternion(), true));
                                break;
                            case 7:
                                UnityEngine.Object foundAsset = null;
                                string name = message.GetString();
                                foreach (UnityEngine.Object uobj in OpenverseClient.Instance.loader.allClientAssets)
                                {
                                    if (name == uobj.name)
                                    {
                                        foundAsset = uobj;
                                    }
                                }
                                if (foundAsset != null)
                                {
                                    properties.Add(varname, new PropertyAssignment(OpenverseClient.Instance.loader.LoadAsset(foundAsset), false));
                                }
                                else
                                {
                                    Debug.LogWarning("Could not find asset with name: " + name);
                                }
                                break;
                            default:
                                properties.Add(varname, null);
                                break;
                        }
                        x++;
                    }

                    //Actually asign the properties
                    foreach (PropertyInfo prop in c.GetType().GetProperties())
                    {
                        if (properties.TryGetValue(prop.Name, out PropertyAssignment assignment) && assignment != null)
                        {
                            if (assignment.isPrimitive)
                            {
                                prop.SetValue(c, assignment.Value);
                            }
                            else
                            {
                                object extracted = ExtractType(assignment.Value, prop.PropertyType);
                                if (extracted != null)
                                    prop.SetValue(c, extracted);
                            }
                            if (!obj.networkedProperties.ContainsKey(c.GetType().Name + "$.$" + prop.Name) && !obj.networkedComponents.ContainsKey(c.name + "$.$" + prop.Name))
                            {
                                obj.networkedProperties.Add(c.GetType().Name + "$.$" + prop.Name, prop);
                                obj.networkedComponents.Add(c.GetType().Name + "$.$" + prop.Name, c);
                            }
                            else
                            {
                                Debug.LogWarning("Variable " + c.GetType().Name + "$.$" + prop.Name + " could not be synced!");
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("Component of type " + index + " was not found! Do you need to update?");
                }
            }
        }

        public static void Spawn(Message message)
        {
            Guid id = Guid.Parse(message.GetString());
            NetworkedObject obj = new GameObject().AddComponent<NetworkedObject>();
            obj.transform.position = message.GetVector3();
            obj.transform.rotation = message.GetQuaternion();
            obj.transform.localScale = message.GetVector3();
            obj.gameObject.name = "(Networked Object) " + message.GetString();
            OpenverseClient.Instance.networkClient.AddObject(id, obj);
        }

        public void UpdateVariable(Message msg)
        {
            string classVarName = msg.GetString();
            PropertyInfo prop = networkedProperties[classVarName];
            Component comp = networkedComponents[classVarName];
            switch (msg.GetUShort())
            {
                case 0:
                    prop.SetValue(comp, msg.GetString());
                    break;
                case 1:
                    prop.SetValue(comp, msg.GetFloat());
                    break;
                case 2:
                    prop.SetValue(comp, msg.GetInt());
                    break;
                case 3:
                    prop.SetValue(comp, msg.GetBool());
                    break;
                case 4:
                    prop.SetValue(comp, msg.GetVector2());
                    break;
                case 5:
                    prop.SetValue(comp, msg.GetVector3());
                    break;
                case 6:
                    prop.SetValue(comp, msg.GetQuaternion());
                    break;
            }
        }

#nullable enable
        private static object? ExtractType(object v, Type type)
        {
            if (v != null)
            {
                if (type == typeof(Mesh))
                {
                    if (v.GetType() == typeof(GameObject))
                    {
                        return ((GameObject)v).GetComponent<MeshFilter>().sharedMesh;
                    }
                    if (v.GetType() == typeof(Mesh))
                    {
                        return (Mesh)v;
                    }
                }
                if (type == typeof(Material))
                {
                    return (Material)v;
                }
            }
            return null;
        }
    }
#nullable disable
    public class PropertyAssignment
    {
        public bool isPrimitive = true;
        public object Value;

        public PropertyAssignment(object value, bool isPrimitive)
        {
            this.Value = value;
            this.isPrimitive = isPrimitive;
        }
    }
}