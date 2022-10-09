# GetNearRankMod

## README for Non-Japanese is [here](README_EN.md)

## はじめに
[GetNearRank](https://github.com/culage/GetNearRank)を参考にした、順位が近くの人が自分よりPPを多く取ったランク曲を取得してプレイリストにするMODです。<br>
Quest2などのスタンドアローン機のみでBeat SaberをしておりPCModが使えない環境の人で、WindowsやMacをお使いの方は、ツール版の[GetNearRankScript](https://github.com/rakkyo150/GetNearRankScript)を利用してください。<br>
GetNearRankScriptも動かすことができない環境の方は、Docker環境があれば使える[GetNearRankDocker](https://github.com/rakkyo150/GetNearRankDocker)をお使いください。<br>

~~現状、日本ローカルランク基準でしか正常に動作しません。<br>
要望などがあれば、ワールドランク基準や他ローカルランク基準でも動作するようにするかもしれません。~~<br>
**v2.9.0から日本以外の方でも使えるようになりました。<br>**
また、グローバルランク基準でも動かせるようになりました(デフォルトはローカルランク基準になっています)。

## 初期設定
~~**初期設定は必須です。**<br>
お手数ですがよろしくお願いします。~~

~~MOD導入後、一度Beat Saberを起動してBeat Saber/UserData/GetNearRankMod.jsonを作成してください。<br>
その後、GetNearRankMod.jsonをメモ帳などで開き、YourIdを自分のScoreSaberのIDに書き換えてください。<br>
ここでのIDとは、自分のScoreSaberのページのURLのhttps://scoresaber.com/u/?????????????????の?部分です。~~<br>

~~例：https://scoresaber.com/u/76561198333869741 の場合、「76561198333869741」に書き換えてください<br>~~

~~次に、YourLocalRankPageNumberに自分が日本何ページ目にいるかを入力してください。<br>
日本１ページ目の人は「1」、日本２ページ目の人は「2」というように書き換えて下さい。<br>
v2.2.0からはページ番号の設定は不要になりました。~~

~~最後に、GetNearRankMod.jsonを上書き保存して初期設定は終了です。~~<br>

v2.3.0から初期設定は不要になりました！！

## 使用方法
ビートセイバーのホーム画面左にあるNEAR RANK PLAYLISTを押すだけです。<br>

1. 押す<br>
![スクリーンショット 2021-10-15 022423](https://user-images.githubusercontent.com/86054813/137366553-a565529a-0d47-4335-a632-029e226efcd6.png)

2. 待つ(メニュー画面から移動しても構いません)<br>
![スクリーンショット 2021-10-15 022232](https://user-images.githubusercontent.com/86054813/137366693-0ab5dbcf-9149-4274-a504-505fa87d4c66.png)

3. リロードが入ったら確認<br>
![スクリーンショット 2021-10-15 022331](https://user-images.githubusercontent.com/86054813/137366817-af0bdbbf-99ed-493d-a31a-3acbdb529f75.png)

v2.4.0からはボタン上に進捗を表示するようになり、エラーが出た場合も"ERROR"と表示されるようになりました。<br>
もしうまくいかない場合は、不具合かもしれないので気軽に報告していただけるととても助かります。<br>
よろしくお願いします。

## 任意設定
Beat Saberのゲーム内ホーム画面の下の歯車→MOD SETTINGS→GetNearRankModから設定を変更できます。<br>
|項目|説明|
|:---|:---|
|`RankRange`|自分の前後何位の人を対象とするか(２ページ分まで情報取得可能)|
|`PPFilter`|何PP差以上を対象とするか|
|`YourPageRange`|自分のトップスコア何ページ目までの情報を取得するか|
|`OthersPageRange`|ライバルのトップスコア何ページ目までの情報を取得するか|
|`GlobalMode`|グローバルランク基準でプレイリストを作成します|
|`FolderMode`|プレイリストをサブフォルダで管理できます|

Beat Saber/UserData/GetNearRankMod.jsonを直接書き換えて変更する設定
|項目|説明|
|:--|:--|
|`FolderName`|サブフォルダ名を変更できます|
