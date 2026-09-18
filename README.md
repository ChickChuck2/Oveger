<div align="center">

# 🚀 Oveger

**Launcher & Organizador de Arquivos Moderno de Alta Performance para Windows**

[![GitHub Release](https://img.shields.io/github/v/release/ChickChuck2/Oveger?style=for-the-badge&logo=semantic-release&color=7952b3)](https://github.com/ChickChuck2/Oveger/releases/latest)
[![Stars](https://img.shields.io/github/stars/ChickChuck2/Oveger?style=for-the-badge&logo=github&color=f1c40f)](https://github.com/ChickChuck2/Oveger/stargazers)
[![Forks](https://img.shields.io/github/forks/ChickChuck2/Oveger?style=for-the-badge&logo=github&color=3498db)](https://github.com/ChickChuck2/Oveger/network/members)
[![Issues Abertas](https://img.shields.io/github/issues/ChickChuck2/Oveger?style=for-the-badge&logo=github&color=e67e22)](https://github.com/ChickChuck2/Oveger/issues)
[![Último Commit](https://img.shields.io/github/last-commit/ChickChuck2/Oveger?style=for-the-badge&logo=git&color=2ecc71)](https://github.com/ChickChuck2/Oveger/commits/master)
[![Tamanho do Repositório](https://img.shields.io/github/repo-size/ChickChuck2/Oveger?style=for-the-badge&logo=files&color=95a5a6)](https://github.com/ChickChuck2/Oveger)
[![Licença MIT](https://img.shields.io/github/license/ChickChuck2/Oveger?style=for-the-badge&logo=open-source-initiative&color=1abc9c)](https://github.com/ChickChuck2/Oveger/blob/master/LICENSE)
[![Plataforma](https://img.shields.io/badge/Platform-Windows%2010%20%7C%2011-0078d4?style=for-the-badge&logo=windows)](https://www.microsoft.com/windows)
[![.NET Framework](https://img.shields.io/badge/.NET-Framework%204.8.1-512bd4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)

<br />

<p align="center">
  <b>Oveger</b> é um overlay translúcido ultra-rápido para Windows desenvolvido em <b>C# / WPF</b>, projetado para revolucionar seu fluxo de trabalho, centralizando acesso a executáveis, diretórios, documentos e mídias sem poluir sua área de trabalho.
</p>

<p align="center">
  <a href="#-showcase">Showcase</a> •
  <a href="#-funcionalidades-core">Funcionalidades</a> •
  <a href="#-arquitetura--tech-stack">Arquitetura</a> •
  <a href="#-como-executar">Como Executar</a> •
  <a href="#-tags--tópicos-do-projeto">Tópicos</a> •
  <a href="#-apoie-o-projeto">Apoie</a> •
  <a href="#-documentação--adrs">Documentação</a>
</p>

</div>

---

## 📺 Showcase

https://github.com/ChickChuck2/Oveger/assets/48648882/4140bc81-2b3c-419f-b24d-147118cce3db

---

## ✨ Funcionalidades Core

- ⚡ **Acesso Instantâneo Global**: Invocação imediata por hotkey customizável (Padrão: <kbd>Ctrl</kbd> + <kbd>Alt</kbd> + <kbd>S</kbd>) com prioridade de processo otimizada.
- 🖥️ **Suporte Multi-Monitor & DPI Aware**: Detecção dinâmica do monitor sob o cursor do mouse e renderização com escala Per-Monitor V2 perfeita.
- 📁 **Organização em Múltiplos Grupos**: Itens e atalhos podem ser associados a múltiplos grupos simultaneamente, organizando categorias complexas com facilidade.
- 🎬 **Pré-visualização Inteligente de Mídia**: Extração e geração assíncrona de thumbnails para vídeos (via FFmpeg) e ícones de alta resolução direto do Shell.
- 🛠️ **Gerenciamento Contextual Completo**: Edição de rótulos, caminhos, permissões e propriedades nativas do Windows pelo menu de contexto.
- 🚀 **Integração Profunda com o Shell**: Inicialização silenciosa (`--minimized`), suporte a System Tray com menu dedicado e inicialização com o Windows.
- 🎨 **Design Glassmorphism Fluido**: Interface escura translúcida inspirada no Fluent Design com micro-animações suaves e foco imersivo.

---

## 🏷️ Tags & Tópicos do Projeto

Explore e acompanhe as áreas temáticas do Oveger nos tópicos oficiais do GitHub:

### 💻 Linguagem & Frameworks
[![C#](https://img.shields.io/badge/csharp-239120?style=flat-square&logo=c-sharp&logoColor=white)](https://github.com/topics/csharp)
[![.NET](https://img.shields.io/badge/dotnet-512BD4?style=flat-square&logo=dotnet&logoColor=white)](https://github.com/topics/dotnet)
[![WPF](https://img.shields.io/badge/wpf-blueviolet?style=flat-square&logo=windows&logoColor=white)](https://github.com/topics/wpf)
[![Win32 API](https://img.shields.io/badge/win32--api-0078D7?style=flat-square&logo=windows&logoColor=white)](https://github.com/topics/win32-api)

### 🎯 Categoria & Produtividade
[![Launcher](https://img.shields.io/badge/launcher-e74c3c?style=flat-square&logo=rocket&logoColor=white)](https://github.com/topics/launcher)
[![App Launcher](https://img.shields.io/badge/app--launcher-e67e22?style=flat-square)](https://github.com/topics/app-launcher)
[![Productivity](https://img.shields.io/badge/productivity-27ae60?style=flat-square&logo=target&logoColor=white)](https://github.com/topics/productivity)
[![File Organizer](https://img.shields.io/badge/file--organizer-f39c12?style=flat-square)](https://github.com/topics/file-organizer)
[![Desktop App](https://img.shields.io/badge/desktop--app-16a085?style=flat-square)](https://github.com/topics/desktop-app)

### 🎨 Experiência & Recursos
[![Overlay](https://img.shields.io/badge/overlay-8e44ad?style=flat-square)](https://github.com/topics/overlay)
[![Glassmorphism](https://img.shields.io/badge/glassmorphism-34495e?style=flat-square)](https://github.com/topics/glassmorphism)
[![Fluent Design](https://img.shields.io/badge/fluent--design-00b4d8?style=flat-square)](https://github.com/topics/fluent-design)
[![Hotkey](https://img.shields.io/badge/hotkey-d35400?style=flat-square)](https://github.com/topics/hotkey)
[![Shortcuts](https://img.shields.io/badge/shortcuts-2980b9?style=flat-square)](https://github.com/topics/shortcuts)
[![Customizable](https://img.shields.io/badge/customizable-9b59b6?style=flat-square)](https://github.com/topics/customizable)

### ⚙️ Plataforma & Licença
[![Windows](https://img.shields.io/badge/windows-0078D6?style=flat-square&logo=windows&logoColor=white)](https://github.com/topics/windows)
[![Windows 11](https://img.shields.io/badge/windows--11-0078D4?style=flat-square&logo=windows-11&logoColor=white)](https://github.com/topics/windows-11)
[![Windows 10](https://img.shields.io/badge/windows--10-0078D6?style=flat-square&logo=windows-10&logoColor=white)](https://github.com/topics/windows-10)
[![Open Source](https://img.shields.io/badge/open--source-brightgreen?style=flat-square&logo=open-source-initiative&logoColor=white)](https://github.com/topics/open-source)

---

## 🛠️ Arquitetura & Tech Stack

Oveger adota arquitetura desacoplada e modular para garantir estabilidade, segurança e tempo de resposta imediato:

```mermaid
graph TD
    A["⌨️ Global Hotkey Hook (Win32)"] --> B["🖥️ MainWindow Overlay"]
    B --> C["📐 Multi-Monitor DPI Handler (SHCore)"]
    B --> D["📂 ConfigManager (JSON Engine)"]
    D --> E["📁 Atalhos & Multi-Grupos"]
    B --> F["🖼️ Icon & Media Manager"]
    F --> G["🎬 NReco FFmpeg (Thumbnails)"]
    F --> H["🪟 Shell32 (Native Icons)"]
    B --> I["Tray Notification Icon"]
```

- **Renderização Acelerada**: Desenvolvido sobre **WPF (.NET Framework 4.8.1)** com Direct3D pipeline para animações suaves.
- **Low-Level Interop**: Utiliza **Win32 API (`user32.dll`, `shell32.dll`, `shcore.dll`)** para registro de hotkeys globais, resolução de DPI dinâmica e interação com o Shell do Windows.
- **Processamento de Vídeo**: Processamento assíncrono via **FFmpeg** (`NReco.VideoConverter`) para criação não-bloqueante de miniaturas de mídias.
- **Armazenamento Seguro**: Formato portátil baseado em **JSON** (`Newtonsoft.Json`), com sincronização automática e proteção de integridade.

---

## 🚀 Como Executar

### Pré-requisitos
- **Windows 10** (1809+) ou **Windows 11**
- **.NET Framework 4.8.1 Runtime**

### Instalação Rápida
1. Baixe o pacote mais recente em [Releases](https://github.com/ChickChuck2/Oveger/releases/latest).
2. Extraia o conteúdo e execute `Oveger.exe`.
3. Pressione <kbd>Ctrl</kbd> + <kbd>Alt</kbd> + <kbd>S</kbd> para abrir o overlay.
4. Adicione itens clicando com o botão direito ou via interface de configurações.

### Compilação a partir do Código-Fonte
```powershell
# Clone o repositório
git clone https://github.com/ChickChuck2/Oveger.git
cd Oveger

# Restaure os pacotes NuGet
nuget restore Oveger.sln

# Compile a versão Release com o MSBuild
msbuild Oveger.sln /p:Configuration=Release /p:Platform="Any CPU"
```

---

## 📂 Documentação & ADRs

O projeto mantém documentação arquitetural contínua:

- 📑 **[Visão Geral Arquitetural](docs/architecture/architecture.md)**: Diagramas, camadas e ciclo de vida do overlay.
- 📐 **[ADR 001: Suporte a Múltiplos Grupos](docs/decisions/adr-001-multi-group-support.md)**: Decisão técnica para categorização N:M.
- 📜 **[Histórico de Versões (Changelog)](CHANGELOG.md)**: Detalhamento de todas as alterações semânticas.
- 📋 **[Diretrizes de Contribuição](.github/ISSUE_TEMPLATE/)**: Formulários padronizados para bugs e novas ideias.

---

## ⭐ Apoie o Projeto

Se o **Oveger** ajuda na sua organização diária ou aumentou sua produtividade, considere apoiar o projeto:

<div align="center">

<a href="https://github.com/ChickChuck2/Oveger/stargazers">
  <img src="https://img.shields.io/badge/⭐%20Deixar%20uma%20Star-GitHub-f1c40f?style=for-the-badge&logo=github" alt="Star no GitHub" />
</a>
&nbsp;
<a href="https://github.com/ChickChuck2/Oveger/network/members">
  <img src="https://img.shields.io/badge/🍴%20Fazer%20um%20Fork-GitHub-3498db?style=for-the-badge&logo=github" alt="Fork no GitHub" />
</a>
&nbsp;
<a href="https://github.com/ChickChuck2/Oveger/issues/new/choose">
  <img src="https://img.shields.io/badge/💡%20Sugerir%20Feature-Issues-2ecc71?style=for-the-badge&logo=github" alt="Criar Issue" />
</a>
&nbsp;
<a href="https://twitter.com/intent/tweet?text=Confira%20o%20Oveger%20-%20Um%20launcher%20e%20organizador%20de%20arquivos%20moderno%20para%20Windows!&url=https://github.com/ChickChuck2/Oveger">
  <img src="https://img.shields.io/badge/📢%20Compartilhar-Twitter%20%2F%20X-1da1f2?style=for-the-badge&logo=x" alt="Compartilhar no X" />
</a>

<br /><br />

> 💬 Tem uma ideia ou encontrou uma inconsistência? Abra uma [Issue](https://github.com/ChickChuck2/Oveger/issues) ou participe das [Discussões](https://github.com/ChickChuck2/Oveger/discussions)!

</div>

---

<div align="center">

Desenvolvido com carinho por **[Carlos Silva (ChickChuck2)](https://github.com/ChickChuck2)** • Licenciado sob [MIT](LICENSE)

</div>
