from playwright.sync_api import sync_playwright, expect
from twocaptcha import TwoCaptcha
import time
import random

# ----------------- Função para resolver o reCAPTCHA -----------------

def resolve_token():
    print('Entrei no resolver Token')

    solver = TwoCaptcha('cb74a7ce31897a1a46e1b17ab762a989')

    sitekey = '6Le1svgrAAAAAIfrGmKLNQm1a1447WBWRMOqyDLd'
    site_url = 'https://sp156.prefeitura.sp.gov.br/portal/servicos/solicitacao?t=559&a=855&servico=1071&anonimo=true'

    token = solver.recaptcha(sitekey=sitekey, url=site_url)  # chamada síncrona
    print("Token recebido:", token['code'])

    return token['code']

# ----------------- Configurações -----------------

# Endereço
endereco_denuncia = "Avenida Paulista, 256"
complemento = "Casa"

# Opcionais
referencia = "Em frente ao Itaú Cultural"
relatar_deficiencia = "Não"
acessibilidade = "Não"   # ou "Sim"

motivo_sim = "Degraus no terreno"
motivo_nao1 = "Pisos Quebrados"
motivo_nao2 = "Buracos"
motivo_nao3 = "Calçada Desnivelada"
motivos_nao = [motivo_nao1, motivo_nao2, motivo_nao3]

descricao = "Estou com problemas para caminhar na calçada"

# Arquivo (ajusta esse caminho se precisar)
arquivos = r"C:\Users\Luiz\Pictures\Screenshots\Captura de tela 2025-09-14 180552.png"

# ----------------- Helper simples de delay humano -----------------

def rnd(a=0.3, b=0.9):
    return random.uniform(a, b)

# ----------------- Função para tratar reCAPTCHA e salvar -----------------

def tratar_recaptcha_e_salvar(page):
    try:
        # 1. Resolver o reCAPTCHA no 2Captcha
        token = resolve_token()

        # 2. Esperar o elemento hidden do reCAPTCHA existir
        page.wait_for_selector('#g-recaptcha-response-1', state="attached", timeout=30000)

        # 3. Injetar o token no campo hidden do reCAPTCHA
        page.evaluate(
            """(token) => { 
                const el = document.getElementById('g-recaptcha-response-1');
                if (el) {
                    el.value = token; 
                    console.log('Token setado no g-recaptcha-response-1');
                } else {
                    console.log('Elemento g-recaptcha-response-1 não encontrado');
                }
            }""",
            token
        )

        # 4. Habilitar o botão de salvar (caso esteja desabilitado)
        page.evaluate("""
            const btn = document.getElementById('btn-solicitacao-salvar');
            if (btn) {
                btn.disabled = false;
                console.log('Botão salvar habilitado');
            } else {
                console.log('Botão salvar não encontrado');
            }
        """)

        print("Estou esperando o botão liberar e o site processar o token...")
        time.sleep(30)

        # 5. Clicar no botão Salvar
        page.click('#btn-solicitacao-salvar')
        print("Cliquei no botão Salvar.")
        time.sleep(2)

    except Exception as e:
        print("Erro ao preencher o reCAPTCHA ou clicar no botão:", e)

    # 6. Esperar um tempo pra ver o resultado / protocolo
    print("Esperando 20 segundos para você ver a tela de confirmação...")
    time.sleep(20)

    # 7. Mantém o navegador aberto até você decidir fechar
    input("Pressione ENTER para fechar o navegador...")


# ----------------- Automação principal -----------------

with sync_playwright() as pw:
    # Mostra o navegador
    browser = pw.chromium.launch(headless=False, slow_mo=0)

    context = browser.new_context()
    page = context.new_page()
    
    # 1. Navegar para a página da prefeitura
    page.goto("https://sp156.prefeitura.sp.gov.br/portal/servicos/solicitacao?servico=931&anonimo=true")
    time.sleep(rnd())

    # 2. Preenchendo Formulário de Endereço
    page.get_by_role("textbox", name="Endereço Completo (Logradouro").click()
    time.sleep(rnd())
    page.get_by_role("textbox", name="Endereço Completo (Logradouro").fill(endereco_denuncia)

    # Simula pressionar seta para baixo e enter (lista seletora)
    time.sleep(rnd())
    page.keyboard.press("ArrowDown")
    
    # Esperando aparecer o seletor
    time.sleep(rnd())
    page.wait_for_selector("#endereco-autocomplete-content", state="visible")
    page.wait_for_selector("#endereco-autocomplete-content li")
    page.click("#endereco-autocomplete-content li:first-child")

    # Conferindo se preencheu o endereço
    enderecoselecionado = page.input_value("#endereco-autocomplete")
    print("Endereço selecionado:", enderecoselecionado)
    
    # Preencher Complemento
    time.sleep(rnd())
    page.get_by_role("textbox", name="Complemento").fill(complemento)

    # 3. Preenchendo Opcionais
    page.get_by_role(
        "textbox",
        name="Indicar ponto de referência se houver (algum"
    ).fill(referencia)

    page.get_by_role(
        "textbox",
        name="Você possui algum tipo de deficiência física "
    ).fill(relatar_deficiencia)

    # Formulário condicional - acessibilidade
    time.sleep(rnd())
    page.select_option("#sl_existem_obstaculos_dificultando_f6c", label=acessibilidade)

    if acessibilidade == "Sim":
        page.wait_for_selector("#sl_qual_o_problema_na_calcada_que__ruf", timeout=8000)
        page.select_option("#sl_qual_o_problema_na_calcada_que__ruf", label=motivo_sim)
        print("Selecionado:", motivo_sim)
    else:
        page.wait_for_selector("#lb_quais_os_motivos_do_pedido_fmj", timeout=8000)
        for motivo in motivos_nao:
            if motivo.strip():
                try:
                    page.locator(f"text={motivo}").click(force=True)
                    print("Marcado:", motivo)
                except Exception as e:
                    print(f"Não foi possível marcar '{motivo}': {e}")

    # Preencher descrição
    time.sleep(rnd())
    page.get_by_role("textbox", name="Descreva detalhes que possam").click()
    page.get_by_role("textbox", name="Descreva detalhes que possam").fill(descricao)

    # Avançar denúncia
    time.sleep(rnd())
    page.get_by_role("button", name="Continuar").click()

    # Upload do arquivo
    time.sleep(3)
    file_input = page.locator("input#anexos-file")
    file_input.set_input_files(arquivos)
    print("Arquivo anexado.")

    # ----------------- PARTE DO CAPTCHA -----------------
    tratar_recaptcha_e_salvar(page)

    # --- Capturar protocolo ---
    try:
        page.wait_for_selector("#numero-protocolo", timeout=15000)
        protocolo = page.inner_text("#numero-protocolo")
        print(f"\n===== PROTOCOLO GERADO: {protocolo} =====\n")

        with open("protocolo.txt", "w", encoding="utf-8") as f:
            f.write(protocolo)

        print("Protocolo salvo em protocolo.txt")

    except Exception as e:
        print("Não consegui capturar o protocolo:", e)

    # Esperar um tempo pra visualizar
    print("Esperando 20 segundos para você ver a tela de confirmação...")
    time.sleep(20)

    input("Pressione ENTER para fechar o navegador...")

    # O navegador só vai fechar DEPOIS que você apertar ENTER lá dentro da função
