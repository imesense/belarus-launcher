# Belarus Launcher

<div>
  <p>
    <a href="./LICENSE.ru.md">
      <img src="https://img.shields.io/badge/License-Non--commercial-red.svg" alt="License" />
    </a>
    <a href="https://github.com/imesense/belarus-launcher/releases/tag/v2.1">
      <img src="https://img.shields.io/github/v/release/imesense/belarus-launcher?include_prereleases&label=Release" alt="Latest release" />
    </a>
    <a href="https://github.com/imesense/belarus-launcher/releases">
      <img src="https://img.shields.io/github/downloads/imesense/belarus-launcher/total?label=Downloads" alt="All downloads" />
    </a>
    <br />
    <a href="https://github.com/imesense/belarus-launcher/actions/workflows/build-launcher.yml">
      <img src="https://github.com/imesense/belarus-launcher/actions/workflows/build-launcher.yml/badge.svg" alt="Build launcher" />
    </a>
    <a href="https://github.com/imesense/belarus-launcher/actions/workflows/build-legacy.yml">
      <img src="https://github.com/imesense/belarus-launcher/actions/workflows/build-legacy.yml/badge.svg" alt="Build legacy" />
    </a>
    <a href="https://github.com/imesense/belarus-launcher/actions/workflows/build-hasher.yml">
      <img src="https://github.com/imesense/belarus-launcher/actions/workflows/build-hasher.yml/badge.svg" alt="Build hasher" />
    </a>
  </p>
</div>

[English](./README.md) | Русский

Специализированный лончер для модификации Беларусь для загрузки и обновления модификации, просмотра текущих новостей и запуска модификации с сервера

![Лончер](./doc/launcher.ru.png)

## Особенности

- **Загрузка** модификации с обновлениями: лончер позволяет легко, быстро загружать модификацию и автоматически проверять наличие обновлений
- **Просмотр** актуальных новостей: игрок всегда будет в курсе последних событий, обновлений и анонсов благодаря лёгкому доступу к текущим новостям
- **Запуск** игры с сервером в удобном интерфейсе

Лончер обеспечивает простоту использования и лёгкий доступ к необходимым функциям, чтобы игрок мог в полной мере насладиться игровым процессом

## Требования

- Visual Studio 2022, Visual Studio Code или Rider
  - Avalonia плагин
- .NET 7 SDK
- Git
- Inno Setup

## Сборка

- Скачать репозиторий:

  ```console
  git clone https://github.com/imesense/belarus-launcher.git
  ```

- Собрать `BelarusLauncher.sln` используя IDE или команду:

  ```console
  dotnet build BelarusLauncher.sln
  ```

## Участники

Все, кто были вовлечены в разработку, перечислены в [этом](./CONTRIBUTORS.ru.md) файле

## Лицензия

Содержимое этого репозитория лицензировано на условиях пользовательской некоммерческой MIT-подобной лицензии, если не указано иное. Подробности смотрите в [этом](./LICENSE.ru.md) файле
