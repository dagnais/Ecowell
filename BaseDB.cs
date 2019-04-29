/*
Titulo: "Ecowell"
Hecho en el año:2018 
-----
Title: "Ecowell"
Made in the year: 2018
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.IO;
using System;

public class BaseDB{
    private string textJson; //Como .json es un archivo de texto aqui se almacena el contenido.
    public JSONNode node; // Utilizamos esta variable para leer como base de datos al archivo .json.

    //Establecemos el contenido del .json actual.
    public void SetCurretText(string txt)
    {
        textJson = txt;
        node= JSON.Parse(textJson);
    }

    //Establecemos el contenido del .json como nodo.
    public void SetCurrentNode(JSONNode currentNode)
    {
        node = currentNode;
    }

    //Guardamos los cambios en la base de datos en la ubicacion especificada
    public void Save(string pathFile)
    {
        //Ejemplo de ubicacion.
        //"C:/DB/Clientes.json"
        File.WriteAllText(pathFile, node.ToString());
    }

    //Cargamos los datos del archivo .json desde la ubicacion especificada.
    public void Load(string pathFile)
    {
        //Ejemplo de ubicacion.
        //"C:/DB/Clientes.json"
        //pathFile= "D:/Animals.json";
        node = JSON.Parse(File.ReadAllText(pathFile));
        Debug.Log("node: " + node.ToString());
        //Descomenta la linea de arriba para ver el contenido que posee la carga de datos.
    }

    //Obtiene la cantidad de entradas que posee la base de datos.
    public int GetAmount()
    {
        //En products dejamos una sugerencia de estructura para hacer la BD .json
        //Si la has seguido podras usar esta funcion en caso contrado deberas modificar el codigo o no usarla.
        return node["Cantidad"].AsInt;
    }

    //Ingresamos los datos a ser guardados en la base de datos.
    public void SaveNewItem(string dataBase, string pathFile,string search, string[] fields, string[] values)
    {
        //Argumentos: nombre de la base de datos, ubicacion , idunico o encabezado de item, campos que contiene 
        //el item , datos a guardar.
        int count = GetAmount();
        //Advertencia para que este codigo funcione debes haber seguido la sugerencia dejada en
        //Products, o tener como primer campo uno llamado index.
        //Este sirve para identificar la fila en la que se encuentra el item.
        node[dataBase][search]["index"] = count.ToString();
        for (int i = 0; i < fields.Length; i++)
        {
            node[dataBase][count][fields[i]] =values[i];
        }
        count++;
        node["Cantidad"] = count.ToString();
        Save(pathFile);
    }

    //Borra una item en la base de datos.
    public void RemoveItem(string dataBase, string pathFile, string seach)
    {
        //Argumentos: nombre de BD, ubicacion , id unico para identificar el item a borrar.
        int codeReplace = node[dataBase][seach]["index"].AsInt;
        if (codeReplace > 0)
        {
            node[dataBase].Remove(seach);

            int count = GetAmount(); 

            count--;

            node["Cantidad"] = count.ToString();

            for (int i = codeReplace; i < node[dataBase].Count; i++)
            {
                node[dataBase][i]["index"] = i.ToString();
            }

            Save(pathFile);
        }
    }

    //Devuelve una serie de strings resultado de una busqueda.
    public string[] ValidateItem(string dataBase, string search, string[] fields)
    {
        //Argumentos: nombre DB, ubicacion , id de busqueda, nombre de los campos en donde se busca.
        int count;
        string[] values;
        //Advertencia para que este codigo funcione debes haber seguido la sugerencia dejada en
        //Products, o tener como primer campo uno llamado index.
        //Este sirve para identificar la fila en la que se encuentra el item.
        count = node[dataBase][search]["index"].AsInt;

        if (count > 0)
        {
            values = new string[fields.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                values[i] = node[dataBase][count][fields[i]].Value;
            }
            return values;
        }
        else
        {
            return null;
        }
    }

    //Realiza una busqueda en toda la base de datos.
    public string[] SearchInDBItem(int i, string dataBase, string[] fields)
    {
        //Argumentos: indicador para utilizarse en estructura repetitiva como for.
        // nombre de la base de datos, nombres de los campos a buscar.
        string[] values;

        values = new string[fields.Length];
        for (int j = 0; j < fields.Length; j++)
        {
            //Debug.Log("db: "+dataBase +" i: "+i + " fl: "+fields[j]);
            values[j] = node[dataBase][i][fields[j]].Value;
        }
        return values;
    }

    //Realiza una busqueda en toda la base de datos sobre el campo "nombre".
    public string[] GetNamesItem(string dataBase)
    {
        //Argumentos: nombre de la base de datos.
        string[] names = new string[node[dataBase].Count - 1];
        for (int i = 1; i < node[dataBase].Count; i++)
        {
            names[i - 1] = node[dataBase][i]["nombre"].Value;
        }
        return names;
    }

    public string[] SearchItem(string dataBase, string search, string[] fields)
    {
        //Argumentos: nombre DB, ubicacion , id de busqueda, nombre de los campos en donde se busca.

        string[] values;
        //Advertencia para que este codigo funcione debes haber seguido la sugerencia dejada en
        //Products, o tener como primer campo uno llamado index.
        //Este sirve para identificar la fila en la que se encuentra el item.
            values = new string[fields.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                values[i] = node[dataBase][search][fields[i]].Value;
            }
            return values;
    }

    public string[] GetIndexByOrder(string dataBase)
    {
        string[] values;
        values = new string[GetAmount()];
        for (int i = 0; i < values.Length; i++)
        {
            //Debug.Log(node[dataBase][i]["index"].Value);
            values[i] = node[dataBase][i]["index"].Value;
        }
        return values;
    }
}
