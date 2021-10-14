# GetNearRankMod

[GetNearRank](https://github.com/culage/GetNearRank)をビートセイバーのゲーム内からでも使えるようにしたMODです。<br>
それに伴って、ゲーム内で見たときのPlaylistの名前を少しわかりやすくもしました。

# 導入方法
前提として[GetNearRank](https://github.com/culage/GetNearRank)が動作する環境が必要です。<br>
[GetNearRank](https://github.com/culage/GetNearRank)をCode->Download ZIPでダウンロードしてください。<br>
ダウンロードしたzipファイルを解凍して、GetNearRank-master以下に少なくともGetNearRank.ps1が存在する形にしてください。<br>
GetNearRank-masterはBeat Saber/Libsにいれて、GetNearRank-master内にreleaseからダウンロードしたGetNearRankMod.batをいれてください。<br>

# 初期設定
GetNearRank.ps1の$MY_URLと$RANK_GET_PAGESはGetNearRank.ps1から直接書き換えてください。<br>
$GET_RANK_RANGEと$PP_FILTERはゲーム内のMod Settings->GetNearRankModやBeat Saber/UserData/GetNearRankMod.jsonから変更することができます。<br>

# 使用方法
ビートセイバーのホーム画面左にあるGENERATE NEAR RANK PLAYLISTを押すだけです。<br>

1. 押す<br>
![スクリーンショット 2021-10-15 022423](https://user-images.githubusercontent.com/86054813/137366553-a565529a-0d47-4335-a632-029e226efcd6.png)

2. 待つ(メニュー画面から移動しても構いません)<br>
![スクリーンショット 2021-10-15 022232](https://user-images.githubusercontent.com/86054813/137366693-0ab5dbcf-9149-4274-a504-505fa87d4c66.png)

3. 確認<br>
![スクリーンショット 2021-10-15 022331](https://user-images.githubusercontent.com/86054813/137366817-af0bdbbf-99ed-493d-a31a-3acbdb529f75.png)

いつまでたってもプレイリストが生成されない場合は、導入方法か初期設定でつまずいている可能性があります。<br>
ビートセイバーを一度終了して、もういちど確認してみてください。<br>
ちなみに、csv.txtはビートセイバーのフォルダに生成されます。<br>

なお、デスクトップでGetNearRankを使用したい場合はGetNearRankMod.batの方ではなくオリジナルのGetNearRank.batを実行してください。<br>
その際、プレイリストはGetNearRank-master内に作成されるので、作成されたプレイリストはご自身でBeat Saber/Playlistsに移動してください。
