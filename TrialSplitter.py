import tkinter as tk
from tkinter import filedialog, messagebox
import pandas as pd
import numpy as np

class TSVReaderGUI:
    def __init__(self, root):
        self.root = root
        root.title("TSV File Reader")

        # File path entry
        self.filepath_entry = tk.Entry(root, width=50)
        self.filepath_entry.grid(row=0, column=0, padx=10, pady=10)

        # Choose file button
        self.choose_button = tk.Button(root, text="Choose", command=self.load_file)
        self.choose_button.grid(row=0, column=1, padx=10, pady=10)

        # Header checkbox
        self.has_header_var = tk.BooleanVar()
        self.has_header_check = tk.Checkbutton(root, text="Has Header", variable=self.has_header_var)
        self.has_header_check.grid(row=1, column=0, columnspan=2)

        # Split files button
        self.split_button = tk.Button(root, text="Split Files by Type", command=self.split_files)
        self.split_button.grid(row=2, column=0, columnspan=2, pady=10)

        # Number of files entry
        self.num_files_entry = tk.Entry(root, width=10)
        self.num_files_entry.grid(row=3, column=0, padx=10, pady=10)

        # Distribute to N files button
        self.distribute_button = tk.Button(root, text="Distribute to N Files", command=self.distribute_files)
        self.distribute_button.grid(row=3, column=1, padx=10, pady=10)

    def load_file(self):
        filepath = filedialog.askopenfilename(filetypes=[("TSV files", "*.tsv"), ("All files", "*.*")])
        if filepath:
            self.filepath_entry.delete(0, tk.END)
            self.filepath_entry.insert(0, filepath)

    def split_files(self):
        filepath = self.filepath_entry.get()
        if not filepath:
            messagebox.showerror("Error", "Please select a file first!")
            return

        header = 0 if self.has_header_var.get() else None
        try:
            df = pd.read_csv(filepath, sep='\t', header=header)
            by_type = df.groupby(df.columns[0])
            for name, group in by_type:
                output_filename = f"{filepath.rsplit('.', 1)[0]}_{name}.tsv"
                group.to_csv(output_filename, sep='\t', index=False, header=True if header == 0 else False)
            messagebox.showinfo("Success", "Files have been split successfully.")
        except Exception as e:
            messagebox.showerror("Error", str(e))

    def distribute_files(self):
        filepath = self.filepath_entry.get()
        try:
            n = int(self.num_files_entry.get())
            df = pd.read_csv(filepath, sep='\t', header=0 if self.has_header_var.get() else None)
            file_dfs = [pd.DataFrame(columns=df.columns) for _ in range(n)]
            type_groups = df.groupby(df.columns[0])

            # Sequential distribution prioritizing types
            for name, group in type_groups:
                rows = group.reset_index(drop=True)  # Reset index for consistent access
                for idx in range(len(rows)):
                    target_df = idx % n
                    file_dfs[target_df] = pd.concat([file_dfs[target_df], rows.iloc[[idx]]], ignore_index=True)

            # Save each DataFrame to a file
            for i, file_df in enumerate(file_dfs):
                output_filename = f"{filepath.rsplit('.', 1)[0]}_Part{i+1}.tsv"
                file_df.to_csv(output_filename, sep='\t', index=False, header=True)
            messagebox.showinfo("Success", "Files have been distributed successfully.")
        except ValueError:
            messagebox.showerror("Error", "Invalid number entered for N.")
        except Exception as e:
            messagebox.showerror("Error", str(e))



def main():
    root = tk.Tk()
    app = TSVReaderGUI(root)
    root.mainloop()

if __name__ == "__main__":
    main()
