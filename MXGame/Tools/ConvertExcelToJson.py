import xlrd
import json
import os
import sys
import collections

ConvertTableNames = {"HeroTable","HeroResTable","SkillTable","EquipTable","HeroGrowTable","HeroSkillTable"}

def write_json_file(json_data,json_file_name):
    with open(json_file_name,'w',encoding='utf-8') as file:
        file.write(json_data)

def convert_to_json(data):
    json_data = json.dumps(data,ensure_ascii=False,indent=4)
    return json_data

def read_excel(current_directory,excel_file_name):
    wb = xlrd.open_workbook(excel_file_name)
    table_names = wb.sheet_names()
    
    for i in range(0,len(table_names)):
        table_name = table_names[i]
        if table_name in ConvertTableNames:
            folder = current_directory + "\\jsons\\"
            
            if not os.path.exists(folder):
                os.makedirs(folder)
            
            json_name = current_directory + "\\jsons\\" + table_name + ".json"
            
            if os.path.exists(json_name):
                os.remove(json_name)
            
            sh = wb.sheet_by_name(table_name)
            row_num = sh.nrows
            
            title = sh.row_values(1)
            
            data = []
            
            for ii in range(2,row_num):
                row_value = sh.row_values(ii)
                single = collections.OrderedDict()
                for column in range(0,len(row_value)):
                    if len(title[column]) > 0 and title[column].find("client") != -1:
                        title_name = title[column].split(':')[0]
                        title_format = title[column].split(':')[1]
                        
                        if title_format == "int" or title_format == "float":
                            if row_value[column] == "":
                                single[title_name] = 0
                            else:
                                if title_format == "int":
                                    single[title_name] = int(row_value[column])
                                else:
                                    single[title_name] = row_value[column]                                                               
                        else:
                            single[title_name] = row_value[column]

                if len(single) > 0:
                    data.append(single)
                
            
            json_data = convert_to_json(data)
            write_json_file(json_data,json_name)
    
        
        

def read_excels():
    current_directory = os.path.dirname(os.path.abspath(__file__))
    excel_filenames = os.listdir(current_directory)
    
    for i in range(len(excel_filenames)):
        if excel_filenames[i].find(".xlsx") != -1:
            excel_name = current_directory + "\\" + excel_filenames[i]
            read_excel(current_directory,excel_name)
    
    
def main():
    read_excels()
    
    
if __name__ == '__main__':
    main()