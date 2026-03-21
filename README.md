# 🚀 Oveger

**Launcher & Organizador de Arquivos Moderno para Windows.**

Oveger é uma solução de produtividade de alto desempenho desenvolvida em **C#/WPF**, projetada para centralizar o acesso a programas, documentos e mídias através de um overlay elegante e personalizável. Focado em eficiência e integração nativa com o ecossistema Windows.

---

## 📺 Showcase

https://github.com/ChickChuck2/Oveger/assets/48648882/4140bc81-2b3c-419f-b24d-147118cce3db

---

## ✨ Funcionalidades Core

- ⚡ **Acesso Global**: Invocação instantânea via hotkey customizável (Padrão: `Ctrl + Alt + S`).
- 📁 **Organização Inteligente**: Categorização de atalhos em grupos expansíveis para um desktop livre de distrações.
- 🎬 **Suporte a Mídia**: Geração automática de thumbnails para vídeos (via FFmpeg) e visualização rápida de imagens.
- 🛠️ **Gestão Nativa**: Edição de rótulos, caminhos e permissões diretamente pela interface.
- 🚀 **Integração de Sistema**: Suporte a System Tray, "Iniciar com Windows" e acesso rápido às propriedades de arquivo via Shell.
- 🎨 **Experiência Visual**: Interface translúcida com animações fluidas e extração de ícones de alta fidelidade.

---

## 🛠️ Arquitetura & Tech Stack

O projeto segue princípios de modularidade para garantir estabilidade e performance:

- **Core Engine**: Desenvolvido em **.NET / WPF** para renderização acelerada por hardware.
- **System Hooks**: Uso intensivo da **Win32 API** para gerenciamento de hotkeys globais e interações de baixo nível com o Shell.
- **Processamento de Mídia**: Integração com **FFmpeg** (via NReco.VideoConverter) para manipulação assíncrona de assets.
- **Persistência**: Camada de dados baseada em **JSON** para configurações rápidas e portáveis.

---

## 🚀 Como Executar

1. Baixe a versão mais recente em [Releases](https://github.com/ChickChuck2/Oveger/releases).
2. Execute `Oveger.exe`.
3. Use **Ctrl + Alt + S** para exibir/ocultar o overlay.
4. Adicione arquivos e programas através do menu de configurações intuitivo.

---

## 📂 Estrutura do Projeto

- `MainWindow.xaml.cs`: Lógica principal de UI e orquestração de eventos.
- `Scripts/`: Módulos especializados em configuração, gestão de ícones e lógica de janelas.
- `Resources/`: Assets visuais e componentes de estilo.

---

Desenvolvido por **Carlos Silva (ChickChuck2)**.
