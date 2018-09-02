# 簡介

**本論文提出一套混合主導設計（Mixed-Initiative Design）系統，此系統稱為戰術創新子系統**。

我們參考動作遊戲中的遊玩特徵（Gameplay Patterns）並對其分類成數個基本戰術類型作為關卡設計師能理解的抽象概念，而系統定義的限制條件在不同的組合下可以對應到不同的基本戰術類型，因此使用者可以選擇欲生成的戰術類型，使系統依據所選的戰術類型而自動生成戰術內容。另外，使用者若想生成不同於這些基本戰術類型的戰術內容，亦可以直接調整相關限制條件與參數，使系統自動生成符合其限制條件與參數的內容。其相關限制條件的內容為調整戰術內容的空間結構和戰術內容的遊戲物件分配。

戰術創新子系統在生成戰術內容的過程中，將欲生成的戰術內容劃分為空間（Space）與遊戲物件（Game Object）結構，而兩種結構的生成方法皆採用基因演算法（Genetic Algorithms）。在生成空間結構的過程中，會依照使用者選擇的相關限制條件而生成最符合其條件的空間結構，接著在生成遊戲物件結構的過程中，會依照空間結構和使用者選擇的相關限制條件而生成最符合其條件的完整戰術內容。最終生成的完整戰術內容便能輸出成戰術檔案，應用於作為本論文後續工作的Ludoscope-like Game Level Design Editor中。

# 戰術創新子系統統流程圖

![framework](https://github.com/AllyChen/NTUST-Ally-Thesis/blob/master/figures/systemframework.png)

# 戰術創新子系統操作介面截圖

功能按鈕分別可以生成戰術內容（Generate）、保留空間結構並重新生成遊戲物件配置（GameObject）和輸出成戰術檔案（Output Data）。相關限制條件與參數列表中的內容為調整戰術內容的空間結構和戰術內容的遊戲物件分配。

![userWindow](https://github.com/AllyChen/NTUST-Ally-Thesis/blob/master/figures/userWindow.png)
