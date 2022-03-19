using RiptideNetworking;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedObject : MonoBehaviour
{
    public Guid myID;

    public static void Spawn(Message message)
    {
        Guid id = Guid.Parse(message.GetString());
        NetworkedObject obj = new GameObject().AddComponent<NetworkedObject>();
        obj.transform.position = message.GetVector3();
        obj.transform.rotation = message.GetQuaternion();
        obj.transform.localScale = message.GetVector3();
        obj.gameObject.name = "(Networked Object) " + message.GetString();
        int componentCount = message.GetInt();
        for (int i = 0; i < componentCount; i++)
        {
            int index = message.GetInt();
            if (AllowedComponents.allowedTypesList.Count > index)
            {
                Type type = AllowedComponents.allowedTypesList[index];
                Component c;
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
                while (x < 500 && message.GetBool())
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
                            foreach (UnityEngine.Object uobj in OpenverseClient.Instance.allClientAssets)
                            {
                                if (name == uobj.name)
                                {
                                    foundAsset = uobj;
                                }
                            }
                            if (foundAsset != null)
                            {
                                properties.Add(varname, new PropertyAssignment(OpenverseClient.Instance.LoadAsset(foundAsset), false));
                            } else
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
                foreach (var prop in c.GetType().GetProperties())
                {
                    if (properties.ContainsKey(prop.Name))
                    {
                        if (properties.ContainsKey(prop.Name) && properties[prop.Name] != null)
                        {
                            if (properties[prop.Name].isPrimitive)
                            {
                                prop.SetValue(c, properties[prop.Name].Value);
                            }
                            else
                            {
                                object extracted = ExtractType(properties[prop.Name].Value, prop.PropertyType);
                                if (extracted != null)
                                    prop.SetValue(c, extracted);
                            }
                        }
                    }
                }
            } else
            {
                Debug.LogWarning("Component of type " + index + " was not found! Do you need to update?");
            }
        }
        OpenverseClient.Instance.AddObject(id, obj);
    }

    private static object ExtractType(object v, Type type)
    {
        if (v != null)
        {
            if (type == typeof(Mesh))
            {
                if (v.GetType() == typeof(GameObject))
                {
                    return ((GameObject)v).GetComponent<MeshFilter>().sharedMesh;
                }
                if(v.GetType() == typeof(Mesh))
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
