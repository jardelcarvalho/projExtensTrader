using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

	class PerfilUsuario {
		
		public const string PORTIFOLIO_KEY = "portifolio";
		public const string RECORD_KEY = "record";
		public const float NAO_EXISTE = float.MinValue;

		public void salvar(string key, float val) {
			if(key == PORTIFOLIO_KEY) {
				PlayerPrefs.SetFloat(PORTIFOLIO_KEY, val);
			} else if(key == RECORD_KEY) {
				PlayerPrefs.SetFloat(RECORD_KEY, val);
			}
			PlayerPrefs.Save();
		}

		public float carrega(string key) {
			if(PlayerPrefs.HasKey(key)) {
				return PlayerPrefs.GetFloat(key);
			} 
			return NAO_EXISTE;
		}

		public void limpaDados() {
			PlayerPrefs.DeleteAll();
		}

	}

	[System.NonSerialized] public static CalculosUteis uteis;
	[SerializeField] private AplicacacaoControlador appControlador;

	[SerializeField] private GameObject dinheiro;
	[SerializeField] private GameObject rendimento;
	[SerializeField] private GameObject lucro;
	[SerializeField] private GameObject unidades;
	[SerializeField] private GameObject SMA1, SMA2, SMA3;
	[SerializeField] private GameObject trendLine;
	[SerializeField] private GameObject records;
	[SerializeField] private GameObject proximo;
	[SerializeField] private GameObject avancar;
	[SerializeField] public GameObject botaoZoom;
	[SerializeField] private GameObject _comprar;
	[SerializeField] private GameObject _vender;
	[SerializeField] private GameObject trocarTema;
	[SerializeField] private GameObject opcoesValor;
	[SerializeField] private GameObject concluirOptVal;
	[SerializeField] private GameObject cancelarOptVal;
	[SerializeField] private GameObject areaLT;
	[SerializeField] private Text ativoAtualTexto;
	[SerializeField] private GameObject telaRecord;
	[SerializeField] private SpriteRenderer[] paineisFundo;
	[SerializeField] private GameObject porcentagemRendimento;
	[SerializeField] private GameObject porcentagemGanho;
	[SerializeField] private InputField campoSma1;
	[SerializeField] private InputField campoSma2;
	[SerializeField] private InputField campoSma3;
	[SerializeField] private Text legSma1;
	[SerializeField] private Text legSma2;
	[SerializeField] private Text legSma3;
	[SerializeField] private GameObject aviso;
	[SerializeField] private Text nomeAtivo;
	[SerializeField] private Text textoB;
	[SerializeField] private GameObject loading;


	private Toggle t_SMA1, t_SMA2, t_SMA3;

	[System.NonSerialized] public bool ativadoSMA1;
	[System.NonSerialized] public bool ativadoSMA2;
	[System.NonSerialized] public bool ativadoSMA3;

	[System.NonSerialized] public bool avancarNoGrafico;
	[System.NonSerialized] public bool proximoGrafico;

	private enum OP : int {NOP = 0, COMPRAR = 1, VENDER = 2};
	private int operacao;

	private float valorEmRendimento = 0;
	private float valorPortifolio = 0;
	private int numeroAtivos = 0;
	private float realPortifolio = 0;
	private float lucroVal = 0f;

	private bool[] estadoAnteriorBotoes = null;

	private float[] valoresPercentuais = new float[] {1, 0.5f, 0.25f, 0.15f, 0.05f};
	private const int DINHEIRO_INICIAL = 20000;

	public float valorAtivoAtual = 0f;

	public void setCor(Color corUIfundo, Color corLetrasPaineis, Color corUIcampos, Color corLetrasCampos, Color corUIbotoes) {
		dinheiro.GetComponent<Image>().color = corUIcampos;
		Text[] textosDinheiro = dinheiro.GetComponentsInChildren<Text>();
		textosDinheiro[0].color = corLetrasCampos;
		textosDinheiro[1].color = corLetrasPaineis;
		rendimento.GetComponent<Image>().color = corUIcampos;
		rendimento.GetComponentInChildren<Text>().color = corLetrasCampos;
		unidades.GetComponent<Image>().color = corUIcampos;
		Text[] textosUnidades = unidades.GetComponentsInChildren<Text>();
		textosUnidades[0].GetComponent<Text>().color = corLetrasCampos;
		textosUnidades[1].GetComponent<Text>().color = corLetrasPaineis;
		lucro.GetComponent<Image>().color = corUIcampos;
		Text[] textosLucro = lucro.GetComponentsInChildren<Text>();
		textosLucro[0].GetComponent<Text>().color = corLetrasCampos;
		textosLucro[1].GetComponent<Text>().color = corLetrasPaineis;
		Image[] imagensSMA = SMA1.GetComponentsInChildren<Image>();
		imagensSMA[0].color = corUIbotoes;
		imagensSMA[1].color = corUIbotoes;
		SMA1.GetComponentInChildren<Text>().color = corLetrasPaineis;

		imagensSMA = SMA2.GetComponentsInChildren<Image>();
		imagensSMA[0].color = corUIbotoes;
		imagensSMA[1].color = corUIbotoes;
		SMA2.GetComponentInChildren<Text>().color = corLetrasPaineis;

		imagensSMA = SMA3.GetComponentsInChildren<Image>();
		imagensSMA[0].color = corUIbotoes;
		imagensSMA[1].color = corUIbotoes;
		SMA3.GetComponentInChildren<Text>().color = corLetrasPaineis;

		trendLine.GetComponent<Image>().color = corUIbotoes;
		records.GetComponent<Image>().color = corUIbotoes;

		_comprar.GetComponent<Image>().color = corUIbotoes;
		_comprar.GetComponentInChildren<Text>().color = Color.black;

		_vender.GetComponent<Image>().color = corUIbotoes;
		_vender.GetComponentInChildren<Text>().color = Color.black;
		
		proximo.GetComponent<Image>().color = corUIbotoes;
		proximo.GetComponentInChildren<Text>().color = Color.black;

		avancar.GetComponent<Image>().color = corUIbotoes;
		avancar.GetComponentInChildren<Text>().color = Color.black;

		trocarTema.GetComponent<Image>().color = corUIbotoes;

		porcentagemGanho.GetComponent<Image>().color = corUIcampos;
		porcentagemRendimento.GetComponent<Image>().color = corUIcampos;

		for(int i = 0; i < paineisFundo.Length; i++) {
			paineisFundo[i].color = corUIfundo;
		}

		Camera.main.backgroundColor = corUIfundo;

	}

	private PerfilUsuario perfil = new PerfilUsuario();
	private float valorRecord;
	void Awake() {
		setInicialBotoes();
		setEventosBotoes();
		realPortifolio = valorPortifolio = perfil.carrega(PerfilUsuario.PORTIFOLIO_KEY);
		if(realPortifolio == PerfilUsuario.NAO_EXISTE) {
			realPortifolio = valorPortifolio = DINHEIRO_INICIAL;
			perfil.salvar(PerfilUsuario.PORTIFOLIO_KEY, realPortifolio);
			perfil.salvar(PerfilUsuario.RECORD_KEY, DINHEIRO_INICIAL);
		}
		valorRecord = perfil.carrega(PerfilUsuario.RECORD_KEY);
		AtualizarDinheiroUI(valorPortifolio);
		AtualizarRendimentoUI(valorEmRendimento);
		AtualizarUnidadesUI(numeroAtivos);
		AtualizarLucroUI(lucroVal);
		textoB.color = Color.black;
	}

	public static void setCalculosObj(CalculosUteis obj) {
		uteis = obj;
	}

	public void setLoading(bool val) {
		loading.SetActive(val);
	}

	public void legendaSetCor(Color a, Color b, Color c) {
		legSma1.color = a;
		legSma2.color = b;
		legSma3.color = c;
		nomeAtivo.text = appControlador.dadosNomeAtivo();
	}

	private void setInicialBotoes() {
		dinheiro.SetActive(true);
		rendimento.SetActive(false);
		unidades.SetActive(false);
		lucro.SetActive(false);
		botaoZoom.SetActive(true);
		areaLT.SetActive(false);
		telaRecord.SetActive(false);

		SMA1.SetActive(true);
		SMA2.SetActive(true);
		SMA3.SetActive(true);
		t_SMA1 = SMA1.GetComponent<Toggle>();
		t_SMA2 = SMA2.GetComponent<Toggle>();
		t_SMA3 = SMA3.GetComponent<Toggle>();

		trendLine.SetActive(true);
		records.SetActive(true);
		proximo.SetActive(true);
		avancar.SetActive(true);

		_comprar.SetActive(true);
		_vender.SetActive(true);

		loading.SetActive(false);
		aviso.SetActive(false);

		porcentagemGanho.SetActive(false);
		porcentagemRendimento.SetActive(false);

		opcoesValor.SetActive(false);

		trocarTema.SetActive(true);

		ativadoSMA1 = false;
		ativadoSMA2 = false;
		ativadoSMA3 = false;

		avancarNoGrafico = false;
		_vender.GetComponent<Button>().interactable = true;
		botaoZoom.GetComponent<Button>().interactable = true;
	}

	private void setEventosBotoes() {
		//Configurando eventos dos botões
		avancar.GetComponent<Button>().onClick.AddListener(Avancar);
		proximo.GetComponent<Button>().onClick.AddListener(Proximo);
		concluirOptVal.GetComponent<Button>().onClick.AddListener(concluir_OpcoesValor);
		cancelarOptVal.GetComponent<Button>().onClick.AddListener(cancelar_OpcoesValor);

		_comprar.GetComponent<Button>().onClick.AddListener(Comprar);
		_vender.GetComponent<Button>().onClick.AddListener(Vender);

	}

	private void interagivel(bool recuperarEstado) {
		if(recuperarEstado) {
			_comprar.GetComponent<Button>().interactable = estadoAnteriorBotoes[0];
			_vender.GetComponent<Button>().interactable = estadoAnteriorBotoes[1];
			SMA1.GetComponent<Toggle>().interactable = estadoAnteriorBotoes[2];
			SMA2.GetComponent<Toggle>().interactable = estadoAnteriorBotoes[3];
			SMA3.GetComponent<Toggle>().interactable = estadoAnteriorBotoes[4];
			records.GetComponent<Button>().interactable = estadoAnteriorBotoes[5];
			trendLine.GetComponent<Button>().interactable = estadoAnteriorBotoes[6];
			trocarTema.GetComponent<Button>().interactable = estadoAnteriorBotoes[7];
			proximo.GetComponent<Button>().interactable = estadoAnteriorBotoes[8];
			avancar.GetComponent<Button>().interactable = estadoAnteriorBotoes[9];
			return;
		}

		estadoAnteriorBotoes = new bool[] { 
			_comprar.GetComponent<Button>().interactable, 
			_vender.GetComponent<Button>().interactable, 
			SMA1.GetComponent<Toggle>().interactable, 
			SMA2.GetComponent<Toggle>().interactable, 
			SMA3.GetComponent<Toggle>().interactable, 
			records.GetComponent<Button>().interactable, 
			trendLine.GetComponent<Button>().interactable, 
			trocarTema.GetComponent<Button>().interactable,
			proximo.GetComponent<Button>().interactable, 
			avancar.GetComponent<Button>().interactable
		};

		_comprar.GetComponent<Button>().interactable = false;
		_vender.GetComponent<Button>().interactable = false;
		SMA1.GetComponent<Toggle>().interactable = false;
		SMA2.GetComponent<Toggle>().interactable = false;
		SMA3.GetComponent<Toggle>().interactable = false;
		records.GetComponent<Button>().interactable = false;
		trendLine.GetComponent<Button>().interactable = false;
		trocarTema.GetComponent<Button>().interactable = false;
		proximo.GetComponent<Button>().interactable = false;
		avancar.GetComponent<Button>().interactable = false;
	}

	private float primeiraPosicao = 0f;
	private bool fstPosi = true;
	public void Avancar() {
		appControlador.proximaEtapaGrafico(); //O novo ativo é adquirido após esta função
		if(valorAtivoAtual < uteis.closeAtual) {
			ativoAtualTexto.color = new Color(71f / 255f, 189f / 255f, 96f / 255f, 1f);
		} else if(valorAtivoAtual > uteis.closeAtual) {
			ativoAtualTexto.color = new Color(189f / 255f, 71f / 255f, 71f / 255f, 1f);
		}
		valorAtivoAtual = uteis.closeAtual;
		ativoAtualTexto.text = formatoDinheiro(valorAtivoAtual);

		if(comprado) {
			valorPosicao = numeroAtivos * valorAtivoAtual;
			lucroVal = valorPosicao - primeiraPosicao;

			AtualizarDinheiroUI(valorPortifolio + lucroVal);
			AtualizarRendimentoUI(valorPosicao);
			AtualizarLucroUI(lucroVal);
			perfil.salvar(PerfilUsuario.PORTIFOLIO_KEY, valorPortifolio + lucroVal);
		} else if(vendido) {
			valorPosicao = numeroAtivos * valorAtivoAtual;
			lucroVal = primeiraPosicao - valorPosicao;

			AtualizarDinheiroUI(valorPortifolio + lucroVal);
			AtualizarRendimentoUI(valorPosicao);
			AtualizarLucroUI(lucroVal);
			perfil.salvar(PerfilUsuario.PORTIFOLIO_KEY, valorPortifolio + lucroVal);
		}

		
		//AtualizarUnidadesUI(numeroAtivos);
		//AtualizarPorcentagens
		//porcentagemGanho.GetComponentInChildren<Text>().text = (lucroVal * (100f / valorEmRendimento)).ToString("N0") + "%";
		porcentagemRendimento.GetComponentInChildren<Text>().text = (numeroAtivos * (100f / ((int)(lucroVal + valorPortifolio) / valorAtivoAtual))).ToString("N0") + "%";

	}

	public void avisoContinuar() {
		appControlador.proximoGrafico();
		valorEmRendimento = 0f;
		lucroVal = 0f;
		porcentagemGanho.SetActive(false);
		porcentagemRendimento.SetActive(false);
		rendimento.SetActive(false);
		lucro.SetActive(false);
		unidades.SetActive(false);
		_comprar.GetComponentInChildren<Text>().text = "Comprado";
		_vender.GetComponentInChildren<Text>().text = "Vendido";
		comprado = vendido = false;
		operacao = (int) OP.NOP;
		interagivel(true);
		aviso.SetActive(false);
		nomeAtivo.text = appControlador.dadosNomeAtivo();

	}

	public void avisoCancelar() {
		interagivel(true);
		aviso.SetActive(false);
	}

	public void Proximo() {
		//Avançar para próximo gráfico
		//mostra msg
		if(valorEmRendimento <= 0) {
			appControlador.proximoGrafico();
			nomeAtivo.text += appControlador.dadosNomeAtivo();
			return;
		}
		aviso.SetActive(true);
		interagivel(false);
		proximoGrafico = true;
		
	}
	
	private void Comprar() {
		opcoesValor.SetActive(!opcoesValor.activeSelf);
		opcoesValor.GetComponentInChildren<Toggle>().isOn = true;
		botaoZoom.SetActive(false);

		interagivel(false);
		
		operacao = (int) OP.COMPRAR;
	}

	private void Vender() {

		opcoesValor.SetActive(!opcoesValor.activeSelf);
		opcoesValor.GetComponentInChildren<Toggle>().isOn = true;
		botaoZoom.SetActive(false);

		interagivel(false);

		operacao = (int) OP.VENDER;
	}

	private bool vendido = false, comprado = false;
	private float valorPosicao = 0f;
	private bool vendido_fst = true;
	private void concluir_OpcoesValor() {
		
		if(operacao == (int) OP.COMPRAR && !comprado && !vendido) {
			comprado = true;
			vendido = false;
			_comprar.GetComponentInChildren<Text>().text = "C. mais";
			_vender.GetComponentInChildren<Text>().text = "Vender";
		} else if(operacao == (int) OP.VENDER && !comprado && !vendido) {
			vendido = true;
			comprado = false;
			_comprar.GetComponentInChildren<Text>().text = "V. mais";
			_vender.GetComponentInChildren<Text>().text = "Encerrar";
			rendimento.SetActive(true);
			unidades.SetActive(true);
			lucro.SetActive(true);
			porcentagemRendimento.SetActive(true);
			AtualizarRendimentoUI(valorPosicao);
			AtualizarUnidadesUI(numeroAtivos);
			AtualizarLucroUI(0);
			porcentagemRendimento.GetComponentInChildren<Text>().text = (numeroAtivos * (100f / (int)(valorPortifolio / valorAtivoAtual))).ToString("N0") + "%";
			_comprar.GetComponent<Button>().interactable = true;
			_vender.GetComponent<Button>().interactable = true;

			primeiraPosicao = valorPosicao;
			fstPosi = false;
		}

		int n = 0, i = 0;
		interagivel(true);

		foreach(Toggle tg in opcoesValor.GetComponentsInChildren<Toggle>()) {
			if(tg.isOn) {
				n = i;
			}
			tg.isOn = false;
			i++;
		}

		if(comprado) {
			if(operacao == (int) OP.COMPRAR) {
				valorPortifolio = perfil.carrega(PerfilUsuario.PORTIFOLIO_KEY);
				float resul = (valorPortifolio - valorPosicao) * valoresPercentuais[n];
				resul -= resul % valorAtivoAtual;
				int qtd = (int) (resul / valorAtivoAtual);
				numeroAtivos += qtd;
				valorPosicao += (float) qtd * valorAtivoAtual;

				//valorPosicao = float.Parse(valorPosicao.ToString("N2"));
				
				rendimento.SetActive(true);
				unidades.SetActive(true);
				lucro.SetActive(true);
				porcentagemRendimento.SetActive(true);
				AtualizarRendimentoUI(valorPosicao);
				AtualizarUnidadesUI(numeroAtivos);
				AtualizarLucroUI(0);
				porcentagemRendimento.GetComponentInChildren<Text>().text = (numeroAtivos * (100f / (int)(valorPortifolio / valorAtivoAtual))).ToString("N0") + "%";
				_comprar.GetComponent<Button>().interactable = true;
				_vender.GetComponent<Button>().interactable = true;

				primeiraPosicao = valorPosicao;
				fstPosi = false;

			} else if(operacao == (int) OP.VENDER) {
				valorPortifolio = perfil.carrega(PerfilUsuario.PORTIFOLIO_KEY);
				float resul = valorPosicao * valoresPercentuais[n];
				//	resul -= resul % valorAtivoAtual;
				resul += valorAtivoAtual - (resul % valorAtivoAtual);
				valorPosicao -= resul;
				int qtd = (int) (resul / valorAtivoAtual);
				numeroAtivos -= qtd;
				primeiraPosicao = valorPosicao;

				AtualizarRendimentoUI(valorPosicao);
				AtualizarUnidadesUI(numeroAtivos);
				AtualizarLucroUI(0);
				porcentagemRendimento.GetComponentInChildren<Text>().text = (numeroAtivos * (100f / ((int) (valorPortifolio + lucroVal) / valorAtivoAtual))).ToString("N0") + "%";

				if(valorPosicao <= valorAtivoAtual) {
					valorPortifolio += lucroVal;
					rendimento.SetActive(false);
					unidades.SetActive(false);
					_vender.GetComponent<Button>().interactable = true;
					_comprar.GetComponentInChildren<Text>().text = "Comprado";
					_vender.GetComponentInChildren<Text>().text = "Vendido";
					porcentagemRendimento.GetComponentInChildren<Text>().text = "0%";
					AtualizarLucroUI(0);
					lucroVal = 0;
					valorPosicao = 0f;
					primeiraPosicao = 0f;
					numeroAtivos = 0;
					lucro.SetActive(false);
					porcentagemGanho.SetActive(false);
					porcentagemRendimento.SetActive(false);
					comprado = false;
					vendido = false;
					fstPosi = true;
				}
			}
		} else if(vendido) {
			if(operacao == (int) OP.COMPRAR || vendido_fst) {
				valorPortifolio = perfil.carrega(PerfilUsuario.PORTIFOLIO_KEY);
				float resul = (valorPortifolio - valorPosicao) * valoresPercentuais[n];
				resul -= resul % valorAtivoAtual;
				int qtd = (int) (resul / valorAtivoAtual);
				numeroAtivos += qtd;
				valorPosicao += (float) qtd * valorAtivoAtual;

				//valorPosicao = float.Parse(valorPosicao.ToString("N2"));
				rendimento.SetActive(true);
				unidades.SetActive(true);
				lucro.SetActive(true);
				porcentagemRendimento.SetActive(true);
				AtualizarRendimentoUI(valorPosicao);
				AtualizarUnidadesUI(numeroAtivos);
				AtualizarLucroUI(0);
				porcentagemRendimento.GetComponentInChildren<Text>().text = (numeroAtivos * (100f / (int)(valorPortifolio / valorAtivoAtual))).ToString("N0") + "%";
				_comprar.GetComponent<Button>().interactable = true;
				_vender.GetComponent<Button>().interactable = true;

				primeiraPosicao = valorPosicao;
				fstPosi = false;
				vendido_fst = false;

			} else if(operacao == (int) OP.VENDER) {
				valorPortifolio = perfil.carrega(PerfilUsuario.PORTIFOLIO_KEY);
				float resul = valorPosicao * valoresPercentuais[n];
				//	resul -= resul % valorAtivoAtual;
				resul += valorAtivoAtual - (resul % valorAtivoAtual);
				valorPosicao -= resul;
				int qtd = (int) (resul / valorAtivoAtual);
				numeroAtivos -= qtd;
				primeiraPosicao = valorPosicao;

				AtualizarRendimentoUI(valorPosicao);
				AtualizarUnidadesUI(numeroAtivos);
				AtualizarLucroUI(0);
				porcentagemRendimento.GetComponentInChildren<Text>().text = (numeroAtivos * (100f / ((int) (valorPortifolio + lucroVal) / valorAtivoAtual))).ToString("N0") + "%";

				if(valorPosicao <= valorAtivoAtual) {
					valorPortifolio += lucroVal;
					rendimento.SetActive(false);
					unidades.SetActive(false);
					_vender.GetComponent<Button>().interactable = true;
					_comprar.GetComponentInChildren<Text>().text = "Comprado";
					_vender.GetComponentInChildren<Text>().text = "Vendido";
					porcentagemRendimento.GetComponentInChildren<Text>().text = "0%";
					AtualizarLucroUI(0);
					lucroVal = 0;
					valorPosicao = 0f;
					primeiraPosicao = 0f;
					numeroAtivos = 0;
					vendido_fst = true;
					lucro.SetActive(false);
					porcentagemGanho.SetActive(false);
					porcentagemRendimento.SetActive(false);
					comprado = false;
					vendido = false;
					fstPosi = true;
				}
			}
		}

		botaoZoom.SetActive(true);
		opcoesValor.SetActive(false);
		operacao = (int) OP.NOP;

		switch(operacao) {
			/*case (int) OP.COMPRAR:
				if(!comprado && !vendido) {
					comprado = true;
					_comprar.GetComponentInChildren<Text>().text = "C. mais";
					_vender.GetComponentInChildren<Text>().text = "Vender";
				}
				


				rendimento.SetActive(true);
				unidades.SetActive(true);
				lucro.SetActive(true);
	
				_comprar.GetComponent<Button>().interactable = true;
				if(valorEmRendimento > 0f) {
					_vender.GetComponent<Button>().interactable = true;
				} 

				AtualizarDinheiroUI(valorPortifolio);
				AtualizarRendimentoUI(valorEmRendimento);
				AtualizarUnidadesUI(numeroAtivos);
				AtualizarLucroUI(lucroVal);
				porcentagemRendimento.GetComponentInChildren<Text>().text = (0).ToString() + "%";
				_vender.GetComponent<Button>().interactable = true;
				porcentagemGanho.SetActive(true);
				porcentagemRendimento.SetActive(true);
			break;

			case (int) OP.VENDER:
				if(!vendido && !comprado) {
					operacao = (int) OP.COMPRAR;
					concluir_OpcoesValor();
					_comprar.GetComponentInChildren<Text>().text = "V. mais";
					_vender.GetComponentInChildren<Text>().text = "Encerrar";
					vendido = true;
					break;
				}




				porcentagemRendimento.GetComponentInChildren<Text>().text = (valorEmRendimento * (100f / valorPortifolio)).ToString("N0") + "%";
				porcentagemGanho.GetComponentInChildren<Text>().text = (lucroVal * (100f / valorEmRendimento)).ToString("N0") + "%";
				_comprar.GetComponent<Button>().interactable = true;
				AtualizarDinheiroUI(valorPortifolio);
				if(valorEmRendimento <= 0f) {
					rendimento.SetActive(false);
					unidades.SetActive(false);
					lucro.SetActive(false);
					_vender.GetComponent<Button>().interactable = true;
					_comprar.GetComponentInChildren<Text>().text = "Comprado";
					_vender.GetComponentInChildren<Text>().text = "Vendido";
					porcentagemRendimento.GetComponentInChildren<Text>().text = "0%";
					porcentagemGanho.GetComponentInChildren<Text>().text = "0%";
					AtualizarLucroUI(0);
					lucroVal = 0;
					porcentagemGanho.SetActive(false);
					porcentagemRendimento.SetActive(false);
					comprado = false;
					vendido = false;
					break;
				}

				AtualizarRendimentoUI(valorEmRendimento);
				AtualizarUnidadesUI(numeroAtivos);

			break;

			case (int) OP.NOP:
				_comprar.GetComponentInChildren<Text>().text = "Comprado";
				_vender.GetComponentInChildren<Text>().text = "Vendido";
				_vender.GetComponent<Button>().interactable = true;
			break;*/
		}

		botaoZoom.SetActive(true);
		opcoesValor.SetActive(false);
		operacao = (int) OP.NOP;

	}

	private void cancelar_OpcoesValor() {
		//Desativa todos os toogles
		foreach(Toggle tg in opcoesValor.GetComponentsInChildren<Toggle>()) {
			tg.isOn = false;
		}

		interagivel(true);

		opcoesValor.SetActive(false);
		operacao = (int) OP.NOP;
		botaoZoom.SetActive(true);
	}

	public void TrocarTema() {
		//Trocar tema
		//appControlador.trocarTema();
		appControlador.espacoCanto();
	}

	public void IncrementarMarcacoesLaterais() {
		//Aumentar numero de marcações laterais
		appControlador.incrementaDivLaterais();
	}

	public void DecrementarMarcacoesLaterais() {
		//Decrementar numero de marcações laterais
		appControlador.decrementaDivLaterais();
	}
 
	private float lastX = float.MaxValue;
	private bool pressionandoZoom = false;
	public void pressionaZoom() {
		pressionandoZoom = true;
	}
	public void naoPressionaZoom() {
		pressionandoZoom = false;
		lastX = float.MaxValue;
	}
	public void zoom() { 
		if(lastX == float.MaxValue) {
			lastX = Input.mousePosition.x;
			return;
		}
		if(Input.mousePosition.x < lastX) {
			appControlador.diminuirElemTela();
		} else if(Input.mousePosition.x > lastX) {
			appControlador.aumentarElemTela();
		}
		lastX = Input.mousePosition.x;
	}


	private bool desenhandoLT = false, dentroArea = false;
	private Vector3 pos1, pos2;
	public void tracandoLT() {
		desenhandoLT = true;
		pos1 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}

	public void dentroAreaLT() {
		dentroArea = true;
	}

	public void naodentroAreaLT() {
		dentroArea = false;
		if(desenhandoLT) {
			naoTracandoLT();
		}
	}

	public void naoTracandoLT() {
		desenhandoLT = false;
		appControlador.tracaLinhaTendencia(new Vector3[2] {pos1, pos2}, false);
		interagivel(true);
		botaoZoom.SetActive(true);
		areaLT.SetActive(false);

	}

	private void desenhaLT() {
		pos2 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		appControlador.tracaLinhaTendencia(new Vector3[2] {pos1, pos2}, true);
	}

	public void LinhaTendencia() {
		//Nova linha de tendencia
		interagivel(false);
		botaoZoom.SetActive(false);
		areaLT.SetActive(true);
	}

	public void Records() {
		//Exibir records
		interagivel(false);
		botaoZoom.SetActive(false);
		areaLT.SetActive(false);
		telaRecord.SetActive(true);
		telaRecord.GetComponentInChildren<Text>().text = formatoDinheiro(valorRecord);
	}

	public void fechaRecords() {
		interagivel(true);
		botaoZoom.SetActive(true);
		areaLT.SetActive(false);
		telaRecord.SetActive(false);
	}

	public void resetaDados() {
		perfil.limpaDados();
		valorPortifolio = realPortifolio = DINHEIRO_INICIAL;
		valorRecord = valorPortifolio;
		perfil.salvar(PerfilUsuario.PORTIFOLIO_KEY, valorPortifolio);
		perfil.salvar(PerfilUsuario.RECORD_KEY, valorRecord);
		telaRecord.GetComponentInChildren<Text>().text = formatoDinheiro(valorRecord);
		fechaRecords();
		AtualizarDinheiroUI(valorPortifolio);
		AtualizarRendimentoUI(valorEmRendimento);
		AtualizarUnidadesUI(numeroAtivos);
		AtualizarLucroUI(lucroVal);
		Records();
	}

	public void ajustaModificacaoValorSma1() {
		legSma1.text = "SMA (" + getNumSMA1() + ")";
		campoSma1.text = getNumSMA1().ToString();
		appControlador.setSMA1(appControlador.SMA1);
	}

	public void ajustaModificacaoValorSma2() {
		legSma2.text = "SMA (" + getNumSMA2() + ")";
		campoSma2.text = getNumSMA2().ToString();
		appControlador.setSMA2(appControlador.SMA2);
	}

	public void ajustaModificacaoValorSma3() {
		legSma3.text = "SMA (" + getNumSMA3() + ")";
		campoSma3.text = getNumSMA3().ToString();
		appControlador.setSMA3(appControlador.SMA3);
	}

	public int getNumSMA1() {
		bool erro = false;
		int n;
		erro = !int.TryParse(campoSma1.text, out n);
		n = erro ? 1 : n;
		return n;
	}

	public int getNumSMA2() {
		bool erro = false;
		int n;
		erro = !int.TryParse(campoSma2.text, out n);
		n = erro ? 1 : n;
		return n;
	}

	public int getNumSMA3() {
		bool erro = false;
		int n;
		erro = !int.TryParse(campoSma3.text, out n);
		n = erro ? 1 : n;
		return n;
	}

	public void toogleSMA1() {
		appControlador.setSMA1(!appControlador.SMA1);
	}

	public void toogleSMA2() {
		appControlador.setSMA2(!appControlador.SMA2);
	}

	public void toogleSMA3() {
		appControlador.setSMA3(!appControlador.SMA3);
	}

	private string formatoDinheiro(float d) {
		string dolares = "$ ";
		return dolares + d.ToString("N2");
	}

	private void AtualizarDinheiroUI(float d) {
		//Atualizar quantidade de dinheiro na caixa de texto da UI
		dinheiro.GetComponentInChildren<Text>().text = formatoDinheiro(d);

	} 

	private void AtualizarRendimentoUI(float d) {
		rendimento.GetComponentInChildren<Text>().text = formatoDinheiro(d); 
	}

	private void AtualizarUnidadesUI(int u) {
		unidades.GetComponentInChildren<Text>().text = u.ToString();
	}

	private void AtualizarLucroUI(float d) {
		//Atualizar quantidade de dinheiro na caixa de texto da UI
		lucro.GetComponentInChildren<Text>().text = formatoDinheiro(d);
	}

	void FixedUpdate() {
		if(pressionandoZoom) {
			zoom();
		} 
		if(desenhandoLT && dentroArea) {
			desenhaLT();
		}	
	}

}
