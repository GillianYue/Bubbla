/*
	CSVReader by Dock. (24/8/11)
	http://starfruitgames.com
 
	usage: 
	CSVReader.SplitCsvGrid(textString)
 
	returns a 2D string array. 
 
	Drag onto a gameobject for a demo of CSV parsing.
*/

using UnityEngine;
using System.Collections;
using System.Linq; 

public class CSVReader : MonoBehaviour 
{

	// outputs the content of a 2D array, useful for checking the importer
	static public void DebugOutputGrid(string[,] grid)
	{
		string textOutput = ""; 
		for (int y = 0; y < grid.GetUpperBound(1); y++) {	
			for (int x = 0; x < grid.GetUpperBound(0); x++) {

				textOutput += grid[x,y]; 
				textOutput += "|"; 
			}
			textOutput += "\n"; 
		}
		print(textOutput);
	}

	// splits a CSV file into a 2D string array
	static public string[,] SplitCsvGrid(string csvText, bool matchLineWithExcel)
	{
		//Debug.Log("before parse: " + csvText);

		string[] lines = csvText.Split('\n'); 

		// finds the max width of row
		int width = 0; 
			string[] firstRow = SplitCsvLine( lines[0] ); 
			width = Mathf.Max(width, firstRow.Length); 

		// creates new 2D string grid to output to
		string[,] outputGrid = new string[width, matchLineWithExcel? lines.Length+1 : lines.Length];

        outputGrid[0, 0] = ""; // row 0 in data has no data, and exists for the sake of consistency between Excel and pointer values";

		for (int r = 0; r < lines.Length; r++)
		{
			string[] row = SplitCsvLine( lines[r] ); 
			for (int c = 0; c < row.Length; c++) 
			{
                if(matchLineWithExcel)
				outputGrid[c,r+1] = row[c]; //the splitCSV result will have different row nums than the output, see above
                else
                outputGrid[c, r] = row[c];
            }
		}

		return outputGrid; 
	}

	// splits a CSV row 
	static public string[] SplitCsvLine(string line)
	{
		return (from System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(line,
			@"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)", 
			System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
			select m.Groups[1].Value).ToArray();
	}
}