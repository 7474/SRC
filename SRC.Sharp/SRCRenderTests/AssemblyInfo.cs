using Microsoft.VisualStudio.TestTools.UnitTesting;

// シナリオを動かすテストは待ち時間が長く CPU を使い切らないため、メソッド単位で並べて動かす。
// 表示言語（プロセス全体の UI カルチャ）を変えるテストは [DoNotParallelize] にする
[assembly: Parallelize(Workers = 0, Scope = ExecutionScope.MethodLevel)]
