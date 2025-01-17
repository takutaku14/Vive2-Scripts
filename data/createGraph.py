import pandas as pd
import matplotlib.pyplot as plt


# CSVファイルの絶対パスリスト
files = [
    'C:/Users/takut/OneDrive/ドキュメント/github/Vive2-Scripts/data/0/LeftContractionList.csv',
    'C:/Users/takut/OneDrive/ドキュメント/github/Vive2-Scripts/data/0/RightContractionList.csv',
    'C:/Users/takut/OneDrive/ドキュメント/github/Vive2-Scripts/data/0/LeftExpantionList.csv',
    'C:/Users/takut/OneDrive/ドキュメント/github/Vive2-Scripts/data/0/RightExpantionList.csv'
]


# グラフの作成
for file in files:
    # CSVファイルの読み込み
    df = pd.read_csv(file)

    # データの確認（オプション）
    print(f"Processing {file}:")
    print(df.head())

    # TimeとDiameterの列を抽出
    time = df['Time']
    diameter = df['Diameter']

    # グラフの作成
    plt.figure(figsize=(10, 6))
    plt.plot(time, diameter, marker='o', linestyle='-')
    plt.title(f'Time vs Diameter for {file.split("/")[-1]}')  # ファイル名をタイトルに
    plt.xlabel('Time')
    plt.ylabel('Diameter')
    plt.grid()

    # グラフを画像ファイルとして保存
    output_file = f'C:/Users/takut/OneDrive/ドキュメント/github/Vive2-Scripts/data/0/{file.split("/")[-1].split(".")[0]}_graph.png'
    plt.savefig(output_file, dpi=300)
    plt.close()  # グラフを閉じる
