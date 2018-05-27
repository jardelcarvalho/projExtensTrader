using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinhaTendencia : MonoBehaviour {

	private class Linha {
		public GameObject linha;
		public float pseudoCloseA;
		public float pseudoCloseB;
		public float xA;
		public float xB;
		public float unidadesDeLarguraRelativoXposiA;
		public float unidadesDeLarguraRelativoXposiB;
	}

	[SerializeField] private GameObject linhaPr;
	[SerializeField] private Transform pontoDestruicao;

	private static float posiYInicial, posiYFinal, posiXInicial, posiXFinal;
	private static CalculosUteis uteis;
	private List<Linha> linhas = new List<Linha>();

	public static void setCalculosObj(CalculosUteis uteis) {
		LinhaTendencia.uteis = uteis;
	}

	public static void setPosY(float posiYInicial, float posiYFinal) {
		LinhaTendencia.posiYInicial = posiYInicial;
		LinhaTendencia.posiYFinal = posiYFinal;
	}

	public static void setPosX(float posiXInicial, float posiXFinal) {
		LinhaTendencia.posiXInicial = posiXInicial;
		LinhaTendencia.posiXFinal = posiXFinal;
	}

	private Color corAtual;
	public void setCor(Color cor) {
		corAtual = cor;
	}

	private Vector3[] pontosTeste = new Vector3[2]; //TESTES

	public void funcaoTeste() {
		pontosTeste[0].x = posiXInicial + 2;
		pontosTeste[0].y = posiYInicial + 2;
		pontosTeste[1].x = posiXFinal - 2;
		pontosTeste[1].y = posiYFinal - 2;
		Debug.Log(pontosTeste[0].z);
		desenha(pontosTeste, true);
	}

	public void deletarTodas() {
		for(int i = 0; i < linhas.Count; i++) {
			Destroy(linhas[i].linha);
		}
		linhas.Clear();
	}

	public IEnumerator atualizaLinhas(int n, bool carregarRetasParaTras) {
		LineRenderer lr;
		Vector3[] pontos = new Vector3[2];
		float largura = uteis.espessura + uteis.ESPACO;
		for(int i = 0; i < linhas.Count; i++) {
			lr = linhas[i].linha.GetComponent<LineRenderer>();
			if(carregarRetasParaTras) {
				linhas[i].xA += n * largura;
				linhas[i].xB += n * largura;
				linhas[i].unidadesDeLarguraRelativoXposiA = (posiXFinal - linhas[i].xA) / (uteis.espessura + uteis.ESPACO);
				linhas[i].unidadesDeLarguraRelativoXposiB = (posiXFinal - linhas[i].xB) / (uteis.espessura + uteis.ESPACO);
			} else {
				linhas[i].xA = posiXFinal - linhas[i].unidadesDeLarguraRelativoXposiA * largura;
				linhas[i].xB = posiXFinal - linhas[i].unidadesDeLarguraRelativoXposiB * largura;
			}
			pontos[0].x = linhas[i].xA;
			pontos[1].x = linhas[i].xB;
			pontos[0].y = posiYInicial + (linhas[i].pseudoCloseA - uteis.menorL) * uteis.alfaHL;
			pontos[1].y = posiYInicial + (linhas[i].pseudoCloseB - uteis.menorL) * uteis.alfaHL;
			if(pontos[0].x < posiXInicial && pontos[1].x < posiXInicial) {
				//Verifica se deve destruir esta reta
				float maiorX = pontos[0].x > pontos[1].x ? pontos[0].x : pontos[1].x;
				linhas[i].linha.SetActive(true);
				if(pontos[0].x < pontoDestruicao.position.x && pontos[1].x < pontoDestruicao.position.x) {	
					Destroy(linhas[i].linha);
					linhas.Remove(linhas[i]);
					continue;
				} else {
					linhas[i].linha.SetActive(false);
					continue;
				}
			}
			linhas[i].linha.SetActive(true);
			pontos[0].z = pontos[1].z = 0f;
			lr.positionCount = 2;
			lr.SetPositions(pontos);
		}
		yield return null;
	}

	private GameObject linhaAtual = null;
	public void desenha(Vector3[] pontos, bool tracando) {
		GameObject go = Instantiate(linhaPr) as GameObject;
		LineRenderer linha = go.GetComponent<LineRenderer>();
		Linha obj = new Linha();
		obj.linha = go;
		obj.xA = pontos[0].x;
		obj.xB = pontos[1].x;
		obj.unidadesDeLarguraRelativoXposiA = (posiXFinal - pontos[0].x) / (uteis.espessura + uteis.ESPACO);
		obj.unidadesDeLarguraRelativoXposiB = (posiXFinal - pontos[1].x) / (uteis.espessura + uteis.ESPACO);
		//Transformando a posição em Y dos pontos em unidades de OHLC.CLOSE (pseudoClose)
		/*Subtraindo p[x].y pela posição inicial da caixa de candles(posiYInicial) tem-se um valor y' que é equivalente a um valor close
		multiplicado pelo seu alfa.
		Dividindo esse valor y' pelo alfa e somando o menorLow(uteis.menorL) tem-se este valor py como unidades de OHLC.close.
		Ou seja, foi feita uma transformação dos valores py para unidades de close*/
		/*O procedimento para fazer os valores de p[x].y variarem de acordo com cada alfa é parecido com o dos candles agora,
		basta pegar as unidade de close que representam p[x].y e subtrair pelo próximo uteis.menorL e multiplicar pelo próximo alfa,
		ao final este valor representa a altura do ponto acima da base da caixa de candles, então, deve-se somar esse valor a 
		posição inicial em Y(posiYInicial) da caixa de candles resultando na nova altura dos valores p[x].y.*/
		obj.pseudoCloseA = (pontos[0].y - posiYInicial) / uteis.alfaHL + uteis.menorL;
		obj.pseudoCloseB = (pontos[1].y - posiYInicial) / uteis.alfaHL + uteis.menorL;
		linha.positionCount = 2;
		linha.startColor = corAtual;
		linha.endColor = corAtual;
		linha.startWidth = 0.05f;
		linha.endWidth = 0.05f;
		pontos[0].z = pontos[1].z = .0f;
		linha.SetPositions(pontos);
		if(linhaAtual != null) {
			Destroy(linhaAtual);
		}
		if(!tracando) {
			linhas.Add(obj);
		} else {
			linhaAtual = obj.linha;
		}
	}
	
}
