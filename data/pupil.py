import pandas as pd
import matplotlib.pyplot as plt

# データを読み込む
data = pd.read_csv(
    r'c:\Users\takut\OneDrive\ドキュメント\github\Vive2-Scripts\data\0\PupilData.CSV')

# 1. Average Diameter のグラフ作成
plt.figure(figsize=(10, 5))
plt.bar(data['Eye'], data['Average Diameter'], color='blue')
plt.title('Average Diameter of Pupil')
plt.xlabel('Eye')
plt.ylabel('Average Diameter')
plt.show()

# 2. Minimum Diameter のグラフ作成
plt.figure(figsize=(10, 5))
plt.bar(data['Eye'], data['Minimum Diameter'], color='green')
plt.title('Minimum Diameter of Pupil')
plt.xlabel('Eye')
plt.ylabel('Minimum Diameter')
plt.show()

# 3. Contraction Rate のグラフ作成
plt.figure(figsize=(10, 5))
plt.bar(data['Eye'], data['Contraction Rate'], color='orange')
plt.title('Contraction Rate of Pupil')
plt.xlabel('Eye')
plt.ylabel('Contraction Rate')
plt.show()

# 4. Contraction Speed のグラフ作成
plt.figure(figsize=(10, 5))
plt.bar(data['Eye'], data['Contraction Speed'], color='red')
plt.title('Contraction Speed of Pupil')
plt.xlabel('Eye')
plt.ylabel('Contraction Speed')
plt.show()

# 5. Expansion Speed のグラフ作成
plt.figure(figsize=(10, 5))
plt.bar(data['Eye'], data['Expansion Speed'], color='purple')
plt.title('Expansion Speed of Pupil')
plt.xlabel('Eye')
plt.ylabel('Expansion Speed')
plt.show()

# 6. Latency のグラフ作成
plt.figure(figsize=(10, 5))
plt.bar(data['Eye'], data['Latency'], color='cyan')
plt.title('Latency of Pupil')
plt.xlabel('Eye')
plt.ylabel('Latency')
plt.show()
