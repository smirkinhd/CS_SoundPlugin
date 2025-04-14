# CS_SoundPlugin
Звуковой плагин для Counter Strike 2
Код, чтобы не засорять репу, лежит в ветке [master](https://github.com/smirkinhd/CS_SoundPlugin/tree/master) (но там нет ничего интересного)

## Описание
У моего друга Ильи есть свой сервер в КС, на котором мы рубимся толпой 6х6, 7х7 и т.д. Там есть всё, но не хватало нам чего-то.
Долгим и томным вечером мой приятель Илья предложил разнообразить наш досуг - написать плагин, который воспроизводит звук при подключении человека на сервер. 

Делать нечего: стали писать. До чего-то дописались (ударения ставьте сами) - 5 часов работы.

## Требования к работе
Необходимые инструменты для корректной работы:
- [CSSharp](https://github.com/roflmuffin/CounterStrikeSharp)
- [Metamod:Source](https://www.sourcemm.net/downloads.php/?branch=master)
- [MultiAddonManager](https://github.com/Source2ZE/MultiAddonManager)

От Ильи (цитирую): "Использовались **MultiAddonManager версии dev build 2.0 - build 1345**. Сам менеджер версии 1.3.5." Далее, он мне зачем то скинул: "**If you are running metamod older than version 1323 you will want to get the pre1323 release instead.**" Мододелы, разберитесь  

## Подводные камни
Плагин работает с MultiAddonManager, т.е. для того, чтобы ваш SoundKit читала игра, вам необходимо разместить в мастерской этот самый SoundKit. В последствии плагин будет читать именно из .vpk ваш аудио файл в формате .vsnd (или .vsnd_c - мы так и не разобрались)

Структура JSON-файла примерно следующая:
```json
{
  "soundMode": "order",
  "musicList": [
    {
      "path": "zvuki/join_sound.vsnd_c"
    }
  ],
  "ConfigVersion": 1
}
```
## Чего ждать в обновлении
Планируется доработать плагин на различные звуки на концовку раунда, на убийство с ножа, на трипл-квадро-пента и более убийства

