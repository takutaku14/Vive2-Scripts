import pandas as pd
import matplotlib.pyplot as plt
import os

# 出力先のディレクトリを指定
output_dir = r'c:\Users\takut\OneDrive\ドキュメント\github\Vive2-Scripts\data\exported'

# ディレクトリが存在しない場合は作成
os.makedirs(output_dir, exist_ok=True)

# データの読み込み
data = []
names = []

for i in range(10):
    # 名前を取得するためのファイルパス
    name_file_path = rf'c:\Users\takut\OneDrive\ドキュメント\github\Vive2-Scripts\data\{i}\name.txt'

    # 名前の読み込み
    try:
        with open(name_file_path, 'r', encoding='utf-8') as name_file:
            name = name_file.read().strip()  # 名前を読み込んで前後の空白を削除
            names.append(name)
    except FileNotFoundError:
        print(f"Error: Name file not found at {name_file_path}")
        names.append("unknown")  # 名前が見つからない場合は"unknown"を使用
    except Exception as e:
        print(f"Error reading the name file: {e}")
        names.append("unknown")  # エラーが発生した場合も"unknown"を使用

    # CSVファイルのパス
    file_path = rf'c:\Users\takut\OneDrive\ドキュメント\github\Vive2-Scripts\data\{i}\LeftAverageList.csv'

    # データの読み込み
    try:
        df = pd.read_csv(file_path)
        data.append((i, df))  # データをタプルにしてリストに追加
    except FileNotFoundError:
        print(f"Error: File not found at {file_path}")
    except Exception as e:
        print(f"Error reading the file: {e}")

# データごとのグラフを作成
colors = ['red', 'blue', 'green', 'orange', 'purple',
          'cyan', 'magenta', 'yellow', 'brown', 'pink']

for index, df in data:
    if 'Time' in df.columns and 'Average Diameter' in df.columns:
        plt.figure(figsize=(10, 6))
        plt.plot(df['Time'], df['Average Diameter'], marker='.',
                 color=colors[index], label=f'data {index}')

        # グラフの装飾
        plt.title(f'LeftAverage - data {index}', fontsize=16)
        plt.xlabel('Time', fontsize=14)
        plt.ylabel('Average Diameter', fontsize=14)
        plt.grid(True, linestyle='--', alpha=0.7)
        plt.legend(fontsize=12)

        # グラフの保存
        plt.savefig(os.path.join(
            output_dir, f'LeftAverage_Data_{index}({names[index]}).png'))
        plt.show()

# まとめたグラフを作成
plt.figure(figsize=(10, 6))

for index, df in data:
    if 'Time' in df.columns and 'Average Diameter' in df.columns:
        plt.plot(df['Time'], df['Average Diameter'], marker='.',
                 color=colors[index], label=f'data {index}')

# まとめたグラフの装飾
plt.title('LeftAverage - All Data', fontsize=16)
plt.xlabel('Time', fontsize=14)
plt.ylabel('Average Diameter', fontsize=14)
plt.grid(True, linestyle='--', alpha=0.7)
plt.legend(fontsize=12)

# まとめたグラフの保存
plt.savefig(os.path.join(output_dir, 'LeftAverage_All.png'))
plt.show()
