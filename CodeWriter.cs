using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

public class CodeWriter {

	private string head;
	private string tail;
	private CodeWriter contain;
	private CodeWriter next;
	public int layer;

	public void output(StringBuilder SB){
		string[] temp;

		string tab = "";
		for (int i = 0; i < layer; i++) {
			tab += "\t";
		}
		temp = head.Split ("\n"[0]);
		for (int i = 0; i < temp.Length; i++) {
			if(temp[i] != "")
				SB.Append (tab + temp[i] + "\n");
		}
		if(contain != null)
			contain.output (SB);
		temp = tail.Split ("\n"[0]);
		for (int i = 0; i < temp.Length; i++) {
			if(temp[i] != "")
				SB.Append (tab + temp[i] + "\n");
		}
		if(next != null)
			next.output (SB);
	}

	public void addHead(string input){
		head += input + "\n";
	}
	public void addHead(string[] input){
		for (int i = 0; i < input.Length; i++) {
			head += input[i] + "\n";
		}
	}
	public void addTail(string input){
		tail = tail + input + "\n";
	}
	public void addTail(string[] input){
		for (int i = 0; i < input.Length; i++) {
			tail = tail + input[i] + "\n";
		}
	}

	public static CodeWriter operator *(CodeWriter A, CodeWriter B){
		CodeWriter temp = A;
		while (temp.contain != null) {
			temp = temp.contain;
		}
		temp.contain = B;
		temp.contain.layer = A.layer + 1;
		return B;//return B for more operate if needed
	}
	public static CodeWriter operator +(CodeWriter A, CodeWriter B){
		CodeWriter temp = A;
		while (temp.next != null) {
			temp = temp.next;
		}
		temp.next = B;
		temp.next.layer = A.layer;
		return A;
	}
	public static CodeWriter operator +(CodeWriter A, string B){
		CodeWriter temp = A;
		while (temp.next != null) {
			temp = temp.next;
		}
		temp.next = new CodeWriter(B);
		temp.next.layer = A.layer;
		return A;
	}
	public CodeWriter (){
		head = "";
		tail = "";
		layer = 0;
	}
	public CodeWriter (string m_head){
		head = m_head + "\n";;
		tail = "";
		layer = 0;
	}
	public CodeWriter (string m_head, string m_tail){
		head = m_head + "\n";
		tail = m_tail + "\n";
		layer = 0;
	}

	public string ToString(){
		return head + tail;
	}
}
