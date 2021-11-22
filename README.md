# GetNearRankMod

[GetNearRank](https://github.com/culage/GetNearRank)を参考にした、順位が近くの人が自分よりPPを多く取ったランク曲を取得してプレイリストにするMODです。<br>
現状、日本ローカルランク基準でしか正常に動作しません。<br>
要望などがあれば、ワールドランク基準や他ローカルランク基準でも動作するようにするかもしれません。

# 初期設定
**初期設定は必須です。**<br>
お手数ですがよろしくお願いします。

MOD導入後、一度Beat Saberを起動してBeat Saber/UserData/GetNearRankMod.jsonを作成してください。<br>
その後、GetNearRankMod.jsonをメモ帳などで開き、YourIdを自分のScoreSaberのIDに書き換えてください。<br>
ここでのIDとは、自分のScoreSaberのページのURLのhttps://scoresaber.com/u/?????????????????の?部分です。<br>

例：https://scoresaber.com/u/76561198333869741 の場合、「76561198333869741」に書き換えてください<br>

~~次に、YourLocalRankPageNumberに自分が日本何ページ目にいるかを入力してください。<br>
日本１ページ目の人は「1」、日本２ページ目の人は「2」というように書き換えて下さい。~~<br>
v2.2.0からはこの設定は不要になりました。

最後に、GetNearRankMod.jsonを上書き保存して初期設定は終了です。

# 使用方法
ビートセイバーのホーム画面左にあるGENERATE NEAR RANK PLAYLISTを押すだけです。<br>

1. 押す<br>
![スクリーンショット 2021-10-15 022423](https://user-images.githubusercontent.com/86054813/137366553-a565529a-0d47-4335-a632-029e226efcd6.png)

2. 待つ(メニュー画面から移動しても構いません)<br>
![スクリーンショット 2021-10-15 022232](https://user-images.githubusercontent.com/86054813/137366693-0ab5dbcf-9149-4274-a504-505fa87d4c66.png)

3. 確認<br>
![スクリーンショット 2021-10-15 022331](https://user-images.githubusercontent.com/86054813/137366817-af0bdbbf-99ed-493d-a31a-3acbdb529f75.png)

いつまでたってもプレイリストが生成されない場合は、初期設定でつまずいている可能性があります。<br>
もう一度確認してみてください。<br>

# 任意設定
Beat Saberのゲーム内ホーム画面の下の歯車→MOD SETTINGS→GetNearRankModから設定を変更できます。<br>
|項目|説明|
|:---|:---|
|`YourLocalRankPageNumber`|日本何ページ目にいるか|
|`RankRange`|自分の前後何位の人を対象とするか(２ページまで情報取得可能)|
|`PPFilter`|何PP差を対象とするか|
|`YourPageRange`|自分のトップスコア何ページ目までの情報を取得するか|
|`OthersPageRange`|ライバルのトップスコア何ページ目までの情報を取得するか|
