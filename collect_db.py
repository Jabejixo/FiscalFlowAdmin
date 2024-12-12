import os

# Укажите путь к директории вашего проекта
project_dir = r"G:\RiderProjects\FiscalFlowAdmin\My models in Wpf"

# Укажите путь и имя выходного файла
output_file = r"G:\RiderProjects\FiscalFlowAdmin\AllCode.txt"

# Список расширений файлов, которые нужно включить
extensions = ['.cs', '.xaml', '.config', '.xml', '.csproj', '.resx']

# Открываем выходной файл для записи
with open(output_file, 'w', encoding='utf-8') as outfile:
    # Обходим директорию проекта рекурсивно
    for root, dirs, files in os.walk(project_dir):
        for file in files:
            # Проверяем, имеет ли файл нужное расширение
            if any(file.endswith(ext) for ext in extensions):
                file_path = os.path.join(root, file)
                with open(file_path, 'r', encoding='utf-8') as infile:
                    # Читаем содержимое файла
                    content = infile.read()
                    # Записываем имя файла в комментарии (опционально)
                    outfile.write(f"// Файл: {file_path}\n")
                    # Записываем содержимое файла в выходной файл
                    outfile.write(content)
                    # Добавляем две пустые строки для разделения файлов
                    outfile.write('\n\n')