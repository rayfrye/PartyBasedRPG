using UnityEngine;
using System;
using System.IO;
using System.Collections;

public class ReadCSV : MonoBehaviour 
{
	
	public string readString(string fileToRead, string dirToSearch)
	{
		string t = "";
		string[] dirs = Directory.GetFiles (dirToSearch);

		foreach(string dir in dirs)
		{
			if(dir == dirToSearch+fileToRead)
			{
				t =  File.ReadAllText(dir);
			}
		}

		return t;
	}

	public string[] readStringArray(string fileToRead, string dirToSearch)
	{
		string[] dirs = Directory.GetFiles (dirToSearch);
		string[] lines_temp = new string[1000];
		int i = 0;

		foreach(string dir in dirs)
		{
			if(dir == dirToSearch+fileToRead)
			{
				lines_temp =  File.ReadAllLines(dir);
			}
		}

		string[] lines = new string[lines_temp.Length];

		for(i = 0; i < lines_temp.Length; i++)
		{
			if(lines_temp[i] != "0")
			{
				lines[i] = lines_temp[i];
			}
		}

		return lines;
	}

	public string[,] readStringMultiDimArray(string fileToRead, string dirToSearch)
	{
		string[] dirs = Directory.GetFiles (dirToSearch);
		string[] lines_temp = new string[0];
		string[,] lines_temp_2 = new string[255,255];
		int maxrow = 0;
		int maxcol = 0;

		foreach(string dir in dirs)
		{
			if(dir == dirToSearch+fileToRead)
			{

				lines_temp = File.ReadAllLines(dir);
			}
		}

		for(int row = 0; row < lines_temp.Length; row++)
		{
			string[] line = lines_temp[row].Split(',');

			for(int col = 0; col < line.Length; col++)
			{
				lines_temp_2[row,col] = line[col];

				if(lines_temp_2[row,col].Length>0)
				{
					maxcol = col+1;
				}
			}
			maxrow = row+1;
		}

		string[,] lines = new string[maxrow,maxcol];

		for(int row = 0; row < maxrow; row ++)
		{
			for(int col = 0; col < maxcol; col++)
			{

				lines[row,col] = lines_temp_2[row,col];
			}
		}

		return lines;
	}

	public bool[,] readBoolMultiDimArray(string fileToRead, string dirToSearch)
	{
		string[] dirs = Directory.GetFiles (dirToSearch);
		string[] lines_temp = new string[0];
		string[,] lines_temp_2 = new string[255,255];
		int maxrow = 0;
		int maxcol = 0;
		
		foreach(string dir in dirs)
		{
			if(dir == dirToSearch+fileToRead)
			{
				lines_temp = File.ReadAllLines(dir);
			}
		}
		
		for(int row = 0; row < lines_temp.Length; row++)
		{
			string[] line = lines_temp[row].Split(',');
			
			for(int col = 0; col < line.Length; col++)
			{
				lines_temp_2[row,col] = line[col];
				
				if(lines_temp_2[row,col].Length>0)
				{
					maxcol = col+1;
				}
			}
			maxrow = row+1;
		}
		
		bool[,] lines = new bool[maxrow,maxcol];
		
		for(int row = 0; row < maxrow; row ++)
		{
			for(int col = 0; col < maxcol; col++)
			{
				if(lines_temp_2[row,col] == "False")
				{
					lines[row,col] = false;
				}

				if(lines_temp_2[row,col] == "True")
				{
					lines[row,col] = true;
				}
			}
		}
		
		return lines;
	}
}
