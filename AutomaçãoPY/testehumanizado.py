# arquivo: automacao_prefeitura.py
from playwright.sync_api import sync_playwright
import time, random, os

# ----------------- Helpers -----------------
def rnd(a=0.08, b=0.25):
    return random.uniform(a, b)

def human_type(page, locator, text):
    """Clica e digita caractere a caractere com delays humanos."""
    locator.click()
    try:
        locator.fill("")  # limpa
    except Exception:
        pass
    for ch in text:
        # delay por caractere entre 40 e 160 ms
        page.keyboard.type(ch, delay=random.randint(40, 160))
    time.sleep(rnd())

def human_move_and_click(page, selector_or_locator):
    """
    Move o mouse ao centro do elemento e clica (suave).
    Aceita string selector ou Locator.
    """
    if isinstance(selector_or_locator, str):
        locator = page.locator(selector_or_locator)
    else:
        locator = selector_or_locator

    # espera estar no DOM (mesmo que oculto)
    locator.wait_for(state="attached", timeout=8000)

    box = None
    try:
        box = locator.bounding_box()
    except Exception:
        box = None

    if box:
        x = box["x"] + box["width"] / 2
        y = box["y"] + box["height"] / 2
        steps = random.randint(6, 18)
        page.mouse.move(x + random.uniform(-3,3), y + random.uniform(-3,3), steps=steps)
        time.sleep(rnd(0.03, 0.12))
        page.mouse.down()
        time.sleep(rnd(0.03, 0.08))
        page.mouse.up()
        time.sleep(rnd())
    else:
        # fallback
        try:
            locator.click(force=True)
        except Exception:
            try:
                page.click(selector_or_locator)
            except Exception:
                pass

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

descricao = "Estou com problemas para caminhhar na calçada"

# Caminho absoluto do arquivo a enviar (ajuste conforme seu PC)
arquivo_absoluto = r"C:\Users\Luiz\Pictures\Screenshots\Captura de tela 2025-09-14 180552.png"
# validação simples
if not os.path.isabs(arquivo_absoluto):
    arquivo_absoluto = os.path.abspath(arquivo_absoluto)

# ----------------- Automação -----------------
with sync_playwright() as pw:
    browser = pw.chromium.launch(headless=False, slow_mo=30)
    context = browser.new_context(viewport={"width": 1280, "height": 800})
    page = context.new_page()

    page.goto("https://sp156.prefeitura.sp.gov.br/portal/servicos/solicitacao?servico=931&anonimo=true")
    time.sleep(rnd())

    # Endereço (autocomplete)
    addr_locator = page.get_by_role("textbox", name="Endereço Completo (Logradouro")
    human_type(page, addr_locator, endereco_denuncia)
    time.sleep(rnd())

    # simula ArrowDown + Enter para acionar sugestão
    page.keyboard.press("ArrowDown")
    time.sleep(rnd(0.15, 0.4))
    page.keyboard.press("Enter")
    # espera apenas o dropdown estar no DOM (pode permanecer oculto)
    try:
        page.wait_for_selector("#endereco-autocomplete-content", state="attached", timeout=8000)
        page.wait_for_selector("#endereco-autocomplete-content li", state="attached", timeout=8000)
        # clica no primeiro item ignorando visibilidade
        page.locator("#endereco-autocomplete-content li:first-child").click(force=True)
    except Exception:
        # fallback: tentar clicar no primeiro li por locator direto (ignorar erros)
        try:
            page.locator("#endereco-autocomplete-content li").first.click(force=True)
        except Exception:
            pass
    time.sleep(rnd())

    # conferir valor
    try:
        enderecoselecionado = page.input_value("#endereco-autocomplete")
        print("Endereço selecionado:", enderecoselecionado)
    except Exception:
        print("Não foi possível ler o valor do input de endereço.")

    # Complemento
    comp_loc = page.get_by_role("textbox", name="Complemento")
    human_type(page, comp_loc, complemento)
    time.sleep(rnd())

    # Referência e deficiência
    ref_loc = page.get_by_role("textbox", name="Indicar ponto de referência se houver (algum")
    human_type(page, ref_loc, referencia)
    time.sleep(rnd())
    def_loc = page.get_by_role("textbox", name="Você possui algum tipo de deficiência física ")
    human_type(page, def_loc, relatar_deficiencia)
    time.sleep(rnd())

    # Seleciona Sim/Não (tenta select_option, senão faz clique humano)
    try:
        page.select_option("#sl_existem_obstaculos_dificultando_f6c", label=acessibilidade)
    except Exception:
        human_move_and_click(page, "#sl_existem_obstaculos_dificultando_f6c")
        time.sleep(rnd())
        human_move_and_click(page, f"xpath=//li[contains(., '{acessibilidade}')]")
    time.sleep(rnd(0.5, 1.0))

    # Fluxos condicionais
    if acessibilidade == "Sim":
        try:
            page.wait_for_selector("#sl_qual_o_problema_na_calcada_que__ruf", state="attached", timeout=8000)
            try:
                page.select_option("#sl_qual_o_problema_na_calcada_que__ruf", label=motivo_sim)
            except Exception:
                human_move_and_click(page, "#sl_qual_o_problema_na_calcada_que__ruf")
                time.sleep(rnd())
                human_move_and_click(page, f"xpath=//li[contains(., '{motivo_sim}')]")
            print("Selecionado:", motivo_sim)
        except Exception:
            print("Campo de problema na calçada não apareceu.")
    else:
        try:
            page.wait_for_selector("#lb_quais_os_motivos_do_pedido_fmj", state="attached", timeout=8000)
            for motivo in motivos_nao:
                if motivo.strip():
                    try:
                        # tenta clicar no label correspondente; usa force para ignorar visibilidade
                        label_locator = page.locator(f"label:has-text('{motivo}')")
                        label_locator.click(force=True)
                        print("Marcado:", motivo)
                        time.sleep(rnd(0.2, 0.6))
                    except Exception as e:
                        # fallback: clicar por texto direto
                        try:
                            page.locator(f"text={motivo}").click(force=True)
                            print("Marcado (fallback):", motivo)
                        except Exception as e2:
                            print(f"Não foi possível marcar '{motivo}': {e2}")
        except Exception:
            print("Bloco de motivos não apareceu no DOM.")

    # Descrição
    try:
        desc_loc = page.get_by_role("textbox", name="Descreva detalhes que possam")
        human_type(page, desc_loc, descricao)
    except Exception:
        print("Campo de descrição não encontrado.")
    time.sleep(rnd())

    # Botão continuar
    try:
        human_move_and_click(page, "button:has-text('Continuar')")
    except Exception:
        try:
            page.get_by_role("button", name="Continuar").click()
        except Exception:
            print("Botão Continuar não encontrado ou não clicável.")
    time.sleep(rnd(1, 2))

    # Upload de arquivo: ignora botão, usa input oculto diretamente
    try:
        file_input = page.locator("input#anexos-file")
        file_input.wait_for(state="attached", timeout=7000)
        # set_input_files aceita string ou lista de strings
        file_input.set_input_files(arquivo_absoluto)
        print("Arquivo enviado com sucesso:", arquivo_absoluto)
    except Exception as e:
        print("Falha ao enviar arquivo via input#anexos-file:", e)
        # tentativa genérica por input[type='file']
        try:
            page.locator("input[type='file']").first.set_input_files(arquivo_absoluto)
            print("Arquivo enviado (fallback input[type='file'])")
        except Exception as e2:
            print("Fallback também falhou:", e2)

    time.sleep(1)

    # Se houver checkbox/label "Não sou um robô", apenas tenta click humano (não contornar)
    try:
        human_move_and_click(page, "text=Não sou um robô")
    except Exception:
        pass

    # espera pra ver resultado
    time.sleep(3)
    browser.close()