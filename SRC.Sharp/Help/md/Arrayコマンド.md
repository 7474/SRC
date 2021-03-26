** 内容はSRC2.2.33のものです **
**Arrayコマンド**

文字列を分割して配列を作成する

**書式**

**Array** *variable string separator*

**指定項目説明**

*variable*作成する配列名

*string*配列を作成する元になる文字列

*separator*分割するのに使われる文字列

**解説**

*string* の内容を*separator* で分割し、配列として*variable* に保存します。

*separator* に"リスト"を指定した場合、*string* をリスト形式の変数として扱い、リストの要素ごとに区切ります。

配列の添字は"1"から始まります。

**例**

Array Var "ABC,DEF,GHI" ","

を実行すると、変数Varの値は

Var[1] = "ABC"

Var[2] = "DEF"

Var[3] = "GHI"

となる。
