using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dados {

	private List<string> diretorios;
	public List<OHLC> dadosAtuais;
	private int dirIni = -1;
	public OHLC[] arrAtual;

	public Dados() {
		diretorios = new List<string>() { "BBAS3Daily", "BBAS3H1", "BBAS3M15", "CMIG4Daily", "CMIG4H1",
		"CMIG4M15", "ITUB4Daily", "ITUB4H1", "ITUB4M15", "OGXP3Daily", "OGXP3H1", "OGXP3M15", "OIBR4Daily",
		"OIBR4H1", "OIBR4M15", "PETR4Daily", "PETR4H1", "PETR4M15", "VALE5Daily", "VALE5H1", "VALE5M15",
		"WEGE3Daily", "WEGE3H1", "WEGE3M15"};
		novaLeitura();
	}

	public string getNomeAtivoAtual() {
		return diretorios[dirIni];
	}

	public void novaLeitura() {
		if(dirIni == -1) {
			dirIni = Random.Range(0, diretorios.Count - 1);
		} else {
			if(dirIni + 1 < diretorios.Count) {
				dirIni++;
			} else {
				dirIni = 0;
			}
		}

		TextAsset lido = Resources.Load(diretorios[dirIni]) as TextAsset;
		if(lido == null || lido.bytes.Length == 0) {
			return;
		}
		System.IO.StreamReader streamReader = new System.IO.StreamReader(new System.IO.MemoryStream(lido.bytes));
		if(dadosAtuais == null) {
			dadosAtuais = new List<OHLC>();
		} else {
			dadosAtuais.Clear();
		}
		string []ohlcStr = null;
		while(!streamReader.EndOfStream) {
			ohlcStr = (streamReader.ReadLine().Split(','));
			dadosAtuais.Add(new OHLC(ohlcStr));
		}
	}

	public OHLC[] retornaVetorOHLCParaGraficoVelas(int indiceAtual, int quantidade) {
		if(quantidade > indiceAtual + 1) {
			return null;
		} 
		return dadosAtuais.GetRange(indiceAtual + 1 - quantidade, quantidade).ToArray();
	}

	public OHLC[] retornaVetorOHLCParaCurvas(int indiceInicial, int quantidade) {
		if(indiceInicial < 0) {
			indiceInicial = 0;
		}
		if(indiceInicial + 1 + quantidade > dadosAtuais.Count + 1) {
			quantidade = dadosAtuais.Count + 1;
		}
		return dadosAtuais.GetRange(indiceInicial, quantidade).ToArray();
	}

}
