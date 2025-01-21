import pandas as pd
import matplotlib.pyplot as plt
import numpy as np
import os

# 出力先のディレクトリを指定
output_dir = r'c:\Users\takut\OneDrive\ドキュメント\github\Vive2-Scripts\data\exported'

# ディレクトリが存在しない場合は作成
os.makedirs(output_dir, exist_ok=True)

# 区別しやすい色のリストを定義
colors = ['red', 'blue', 'green', 'orange', 'purple',
          'cyan', 'magenta', 'yellow', 'brown', 'pink']

# 0から9までの繰り返し
for i in range(10):
    # 名前を取得するためのファイルパス
    name_file_path = rf'c:\Users\takut\OneDrive\ドキュメント\github\Vive2-Scripts\data\{i}\name.txt'

    # 名前の読み込み
    try:
        with open(name_file_path, 'r', encoding='utf-8') as name_file:
            name = name_file.read().strip()  # 名前を読み込んで前後の空白を削除
    except FileNotFoundError:
        print(f"Error: Name file not found at {name_file_path}")
        name = "unknown"  # 名前が見つからない場合は"unknown"を使用
    except Exception as e:
        print(f"Error reading the name file: {e}")
        name = "unknown"  # エラーが発生した場合も"unknown"を使用

    # CSVファイルのパス
    file_path = rf'c:\Users\takut\OneDrive\ドキュメント\github\Vive2-Scripts\data\{i}\RightContractionList.csv'

    # データの読み込み
    try:
        data = pd.read_csv(file_path)
    except FileNotFoundError:
        print(f"Error: File not found at {file_path}")
        continue  # 次のループへ
    except Exception as e:
        print(f"Error reading the file: {e}")
        continue  # 次のループへ

    # 必要なカラムが存在するか確認
    if 'Time' not in data.columns or 'Diameter' not in data.columns:
        print("Error: Required columns 'Time' or 'Diameter' are missing in the CSV file.")
        continue  # 次のループへ

    # 各データセットのグラフを作成
    plt.figure(figsize=(10, 6))
    plt.scatter(data['Time'], data['Diameter'],
                label=f'Data {i}', color=colors[i], marker='.')

    # グラフの装飾
    plt.title(f'RightContraction - Data {i}', fontsize=16)
    plt.xlabel('Time', fontsize=14)
    plt.ylabel('Diameter', fontsize=14)
    plt.grid(True, linestyle='--', alpha=0.7)
    plt.legend(fontsize=12)

    # グラフの保存
    plt.savefig(os.path.join(
        output_dir, f'RightContraction_data_{i}({name}).png'))  # データ番号,名前を含む画像名で保存
    plt.show()

# すべてのデータをまとめた散布図を作成
plt.figure(figsize=(10, 6))

# もう一度0から9までの繰り返し
for i in range(10):
    # 名前を取得するためのファイルパス
    name_file_path = rf'c:\Users\takut\OneDrive\ドキュメント\github\Vive2-Scripts\data\{i}\name.txt'

    # 名前の読み込み
    try:
        with open(name_file_path, 'r', encoding='utf-8') as name_file:
            name = name_file.read().strip()  # 名前を読み込んで前後の空白を削除
    except FileNotFoundError:
        print(f"Error: Name file not found at {name_file_path}")
        name = "unknown"  # 名前が見つからない場合は"unknown"を使用
    except Exception as e:
        print(f"Error reading the name file: {e}")
        name = "unknown"  # エラーが発生した場合も"unknown"を使用

    # CSVファイルのパス
    file_path = rf'c:\Users\takut\OneDrive\ドキュメント\github\Vive2-Scripts\data\{i}\RightContractionList.csv'

    # データの読み込み
    try:
        data = pd.read_csv(file_path)
    except FileNotFoundError:
        print(f"Error: File not found at {file_path}")
        continue  # 次のループへ
    except Exception as e:
        print(f"Error reading the file: {e}")
        continue  # 次のループへ

    # 各データの散布図をまとめる
    plt.scatter(data['Time'], data['Diameter'],
                label=f'Data {i}', color=colors[i], marker='.')

# まとめたグラフの装飾
plt.title('RightContraction - All Data', fontsize=16)
plt.xlabel('Time', fontsize=14)
plt.ylabel('Diameter', fontsize=14)
plt.grid(True, linestyle='--', alpha=0.7)
plt.legend(fontsize=12)

# まとめたグラフの保存
plt.savefig(os.path.join(
    output_dir, 'RightContraction_all.png'))  # まとめた画像名で保存
plt.show()
