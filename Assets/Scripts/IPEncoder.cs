using System;
					
public class IPEncoder
{
	public static string EncodeToBase36(string ip)
	{
		string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		string[] stringNumbers = ip.Split('.');
		int[] intNumbers = new int[4];
		int index = 0;
		foreach(string stringNumber in stringNumbers)
		{
			intNumbers[index] = Convert.ToInt32(stringNumber);
			index++;
		}
		
		string code = "";
		index = 0;
		foreach(int intNumber in intNumbers)
		{
			if(intNumber < 36)
			{
				code += "0"+chars[intNumber];
			}
			else
			{
				code += ""+chars[intNumber/36]+chars[intNumber%36];
			}
		}
		return code;
	}
	
	public static string DecodeFromBase36(string code)
	{
		string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		return ""+(chars.IndexOf(code[0])*36+chars.IndexOf(code[1]))+"."+
			(chars.IndexOf(code[2])*36+chars.IndexOf(code[3]))+"."+
			(chars.IndexOf(code[4])*36+chars.IndexOf(code[5]))+"."+
			(chars.IndexOf(code[6])*36+chars.IndexOf(code[7]));
	}
}