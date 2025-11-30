import os
import json
import random
import time
import asyncio

from twocaptcha import TwoCaptcha
from playwright.async_api import async_playwright



data= r"data\info.json"

img_dir = "img"
arquivos = os.listdir(img_dir)
arquivo = os.path.join(img_dir, arquivos[0])

with open(data, "r", encoding="utf-8") as f:
    info = json.load(f)

# ----------------- Helpers -----------------
def rnd(a=0.08, b=0.25):
    return random.uniform(a, b)

async def human_type(page, locator, text):
    """Clica e digita caractere a caractere com delays humanos."""
    locator.click()
    try:
        await locator.fill("")  # limpa
    except Exception:
        pass
    for ch in text:
        # delay por caractere entre 40 e 160 ms
        await page.keyboard.type(ch, delay=random.randint(40, 160))
    time.sleep(rnd())

async def human_move_and_click(page, selector_or_locator):
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

async def resolve_token():

    print('Entrei no resolver Token')

    solver = TwoCaptcha('cb74a7ce31897a1a46e1b17ab762a989')

    sitekey = '6Le1svgrAAAAAIfrGmKLNQm1a1447WBWRMOqyDLd'

    site_url = 'https://sp156.prefeitura.sp.gov.br/portal/servicos/solicitacao?t=559&a=855&servico=1071&anonimo=true'

    token = solver.recaptcha(sitekey=sitekey, url=site_url)

    print(token['code'])

    return token['code']




# ----------------- Automação -----------------

async def automation():
    async with async_playwright() as pw:
        browser = await pw.chromium.launch(headless=False, slow_mo=30)
        context = await browser.new_context(viewport={"width": 1280, "height": 800})
        page = await context.new_page()

        await page.goto("https://sp156.prefeitura.sp.gov.br/portal/servicos/solicitacao?t=799&a=855&servico=1071&anonimo=true")
        time.sleep(rnd())

        # Endereço (autocomplete)
        addr_locator = page.get_by_role("textbox", name="Endereço Completo (Logradouro")
        end = info["end_den"]
        await human_type(page, addr_locator, end)
        time.sleep(rnd())

        # simula ArrowDown + Enter
        await page.keyboard.press("ArrowDown")
        time.sleep(rnd(0.15, 0.4))
        await page.keyboard.press("Enter")

        try:
            await page.wait_for_selector("#endereco-autocomplete-content", state="attached", timeout=8000)
            await page.wait_for_selector("#endereco-autocomplete-content li", state="attached", timeout=8000)
            await page.locator("#endereco-autocomplete-content li:first-child").click(force=True)
        except Exception:
            try:
                await page.locator("#endereco-autocomplete-content li").first.click(force=True)
            except Exception:
                pass

        time.sleep(rnd())

        try:
            enderecoselecionado = await page.input_value("#endereco-autocomplete")
            print("Endereço selecionado:", enderecoselecionado)
        except Exception:
            print("Não foi possível ler o valor do input de endereço.")

        # -----------------------------
        # Campos específicos
        # -----------------------------

        # Calçada
        try:
            await page.wait_for_selector('#sl-arvore-externa', state="attached", timeout=8000)
            await page.locator('#sl-arvore-externa').select_option("Sim" if info["loc_arv"] else "Não")
        except Exception as e:
            print(f"Tipo de calçada erro {e}")

        # Motivo
        try:
            await page.wait_for_selector('#ip-motivo-solicitacao', state="attached", timeout=8000)
            await page.locator("#ip-motivo-solicitacao").fill(info["mot_solic"])
        except Exception as e:
            print(f"Erro motivo: {e}")

        # Contato rede elétrica
        try:
            await page.wait_for_selector('#sl-rede-eletricaa', state="attached", timeout=8000)
            if info["rede_elet"]:
                await page.locator('#sl-rede-eletricaaa').select_option("Sim")
            else:
                await page.locator('#sl-rede-eletricaa').select_option("Não")
        except Exception as e:
            print(f"Rede elétrica erro: {e}")

        # Cupins
        try:
            await page.wait_for_selector('#sl-cupim-ou-oco', state="attached", timeout=8000)
            await page.locator('#sl-cupim-ou-oco').select_option("Sim" if info["cupins"] else "Não")
        except Exception as e:
            print(f"Cupins erro: {e}")

        # Árvore inclinada
        try:
            await page.wait_for_selector('#sl-arvore-inclinada', state="attached", timeout=8000)
            if info["risc"]:
                await page.locator('#sl-arvore-inclinada').select_option("Sim")
                try:
                    await page.wait_for_selector('#txt-inclinacao-aumentou', state="attached", timeout=8000)
                    await page.locator("#txt-inclinacao-aumentou").fill(info["ind_risc"])
                except Exception as e:
                    print(f"Inclinação erro: {e}")
            else:
                await page.locator('#sl-arvore-inclinada').select_option("Não")
        except Exception as e:
            print(f"Erro árvore inclinada: {e}")

        # Queda eminente
        try:
            await page.wait_for_selector('#sl-risco-queda', state="attached", timeout=8000)
            if info["risc_imi"]:
                await page.locator('#sl-risco-queda').select_option("Sim")
                try:
                    await page.wait_for_selector('#ip-indicador-risco', state="attached", timeout=8000)
                    await page.locator("#ip-indicador-risco").fill(info["mot_solic"])
                except Exception as e:
                    print(f"Indicador risco erro: {e}")
            else:
                await page.locator('#sl-risco-queda').select_option("Não")
        except Exception as e:
            print(f"Risco eminente erro: {e}")

        # Atingida
        try:
            await page.wait_for_selector('#sl-arvore-danificada', state="attached", timeout=8000)
            await page.locator('#sl-arvore-danificada').select_option("Sim" if info["risc_dmg"] else "Não")
        except Exception as e:
            print(f"Atingida erro {e}")

        # Descrição
        try:
            desc_loc = page.get_by_placeholder("Descreva detalhes que possam")
            await human_type(page, desc_loc, info['desc'])
        except Exception as e:
            print(f"Erro descrição: {e}")

        time.sleep(5)

        # Continuar
        try:
            await page.locator("#btn-solicitacao-continuar").click()
        except Exception:
            try:
                await page.get_by_role("button", name="Continuar").click()
            except Exception:
                print("Botão Continuar não clicável.")

        time.sleep(rnd(1, 2))

        # Upload
        try:
            file_input = page.locator("input#anexos-file")
            await file_input.wait_for(state="attached", timeout=7000)
            await file_input.set_input_files(arquivo)
        except Exception:
            try:
                await page.locator("input[type='file']").first.set_input_files(arquivo)
            except Exception as e:
                print("Upload falhou:", e)

        time.sleep(5)

        # Recaptcha fake
        try:

            token = await resolve_token() 

            await page.wait_for_selector('#g-recaptcha-response-1', state="attached")

            await page.evaluate(f"""document.getElementById('g-recaptcha-response-1').value = "{token}";""")

            await page.evaluate("""
                const btn = document.getElementById('btn-solicitacao-salvar');
                btn.disabled = false;
            """)

            print("Estou esperando o botão liberar")

            time.sleep(30)
            await page.click('#btn-solicitacao-salvar')
            time.sleep(2)

        except Exception as e:
            print(f"Erro em algum lugar {e}")

        await asyncio.sleep(99999)

        await browser.close()



asyncio.run(automation())