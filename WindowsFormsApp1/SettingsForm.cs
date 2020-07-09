using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class SettingsForm : Form
    {
        private Dictionary<PropertyInfo, (Label, Control)> _inputs;
        private object _settings;


        public SettingsForm(object settings)
        {
            _settings = settings;
        }


        public void ShowWindow() {
            _inputs = new Dictionary<PropertyInfo, (Label, Control)>();
            int cursorY = 10;
            int cursorX = 10;
            int maxLabelWidth = 0;
            int maxComponentWidth = 0;

            foreach (PropertyInfo prop in _settings.GetType().GetProperties())
            {
                var attr = (SettingsAttribute)Attribute.GetCustomAttribute(prop, typeof(SettingsAttribute));
                string name = prop.Name;
                if (attr != null)
                {
                    name = attr.DisplayName;
                }
                if (name.Length < 1)
                    continue;
                Type type = prop.PropertyType;
                int labelWidth = TextRenderer.MeasureText(name, Font).Width;
                Label label = new Label()
                {
                    Text = name,
                    Location = new Point(cursorX, cursorY),
                    Width = labelWidth
                };
                if (labelWidth > maxLabelWidth)
                {
                    maxLabelWidth = labelWidth;
                }
                Controls.Add(label);
                Control component;
                if (type == typeof(string))
                {
                    component = new TextBox()
                    {
                        Text = (string)prop.GetValue(_settings),
                        Location = new Point(cursorX + labelWidth + 10)
                    };
                }
                else if (type == typeof(int))
                {
                    component = new NumericUpDown()
                    {
                        Value = (int)prop.GetValue(_settings),
                        Location = new Point(cursorX + labelWidth + 10, cursorY)
                    };
                }
                else if (type == typeof(double))
                {
                    component = new TextBox()
                    {
                        Text = prop.GetValue(_settings).ToString(),
                        Location = new Point(cursorX + labelWidth + 10)
                    };
                }
                else if (type.IsEnum)
                {
                    ComboBox box = new ComboBox()
                    {
                        Location = new Point(cursorX + labelWidth + 10, cursorY)
                    };
                    foreach (object item in Enum.GetValues(type))
                    {
                        box.Items.Add(item);
                    }
                    box.SelectedItem = prop.GetValue(_settings);
                    component = box;
                }
                else if (type == typeof(Color))
                {

                    TextBox color = new TextBox()
                    {
                        ReadOnly = true,
                        BackColor = (Color)prop.GetValue(_settings),
                        Width = label.Height,
                        Height = label.Height,
                        Location = new Point(0, 0)
                    };


                    Button button = new Button()
                    {
                        Text = "Change",
                        Location = new Point(label.Height + 10, 0)
                    };
                    button.Click += (sender, args) =>
                    {
                        ColorDialog dialog = new ColorDialog();
                        dialog.Color = (Color)prop.GetValue(_settings);
                        DialogResult result = dialog.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            color.BackColor = dialog.Color;
                        }
                    };

                    component = new Panel()
                    {
                        Location = new Point(cursorX + labelWidth + 20, cursorY),
                        Width = color.Width + 10 + button.Width,
                        Height = button.Height
                    };
                    component.Controls.Add(color);
                    component.Controls.Add(button);

                }
                else if (type == typeof(DateTime))
                {
                    DateTimePicker picker = new DateTimePicker()
                    {
                        Location = new Point(cursorX + labelWidth + 20, cursorY)
                    };
                    picker.Value = (DateTime)prop.GetValue(_settings);
                    component = picker;
                }
                else if (type == typeof(Font)) {
                    Font currentValue = (Font)prop.GetValue(_settings);
                    string fontDescription = GetFontDescription(currentValue);
                    Label fontLabel = new Label()
                    {
                        Text = fontDescription,
                        Location = new Point(0, 0),
                        Width = TextRenderer.MeasureText(fontDescription, Font).Width
                    };
                    Button fontButton = new Button()
                    {
                        Text = "Change",
                        Location = new Point(fontLabel.Width + 15, 0)
                    };

                    component = new Panel()
                    {
                        Location = new Point(cursorX + labelWidth + 20, cursorY),
                        Width = fontLabel.Width + fontButton.Width + 15
                    };
                    fontButton.Click += (sender, args) => {
                        FontDialog dialog = new FontDialog();
                        dialog.Font = currentValue;
                        DialogResult result = dialog.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            fontLabel.Text = GetFontDescription(dialog.Font);
                            fontLabel.Width = TextRenderer.MeasureText(fontLabel.Text, dialog.Font)
                                .Width;
                            fontButton.Location = new Point(fontLabel.Location.X +
                                fontLabel.Width + 15, fontButton.Location.Y);
                            component.Width = fontButton.Width + fontLabel.Width + 15;
                        }
                    };
                    component.Controls.Add(fontLabel);
                    component.Controls.Add(fontButton);
                }
                else
                {
                    continue;
                }
                cursorY += component.Height + 20;
                Controls.Add(component);
                _inputs[prop] = (label, component);
            }

            foreach (var pair in _inputs)
            {
                (Label l, Control c) = pair.Value;
                l.Width = maxLabelWidth;
                c.Location = new Point(l.Width + 25, c.Location.Y);
            }

            Button saveButton = new Button()
            {
                Text = "Save",
            };
            Button cancelButton = new Button()
            {
                Text = "Cancel"
            };

            int totalWidth = saveButton.Width + cancelButton.Width + 25;
            int x = (Width - totalWidth) / 2;
            saveButton.Location = new Point(x, cursorY);
            cancelButton.Location = new Point(x + saveButton.Width + 25, cursorY);
            saveButton.Click += (sender, args) => {
                foreach (var pair in _inputs) {
                    if (pair.Key.PropertyType == typeof(string))
                    {
                        TextBox textBox = (TextBox)pair.Value.Item2;
                        pair.Key.SetValue(_settings, textBox.Text);
                    }
                    else if (pair.Key.PropertyType == typeof(double))
                    {
                        TextBox textBox = (TextBox)pair.Value.Item2;
                        double value;
                        if (double.TryParse(textBox.Text, out value))
                        {
                            pair.Key.SetValue(_settings, value);
                        }
                        else
                        {
                            MessageBox.Show("Some values could not be saved.", "Invalid input");
                        }
                    }
                    else if (pair.Key.PropertyType == typeof(int))
                    {
                        NumericUpDown upDown = (NumericUpDown)pair.Value.Item2;
                        pair.Key.SetValue(_settings, upDown.Value);
                    }
                    else if (pair.Key.PropertyType.IsEnum)
                    {
                        ComboBox combo = (ComboBox)pair.Value.Item2;
                        pair.Key.SetValue(_settings, combo.SelectedItem);
                    }
                    else if (pair.Key.PropertyType == typeof(Color))
                    {
                        Panel panel = (Panel)pair.Value.Item2;
                        foreach (Control c in panel.Controls)
                        {
                            if (c is TextBox)
                            {
                                TextBox box = (TextBox)c;
                                pair.Key.SetValue(_settings, box.BackColor);
                            }
                        }
                    }
                    else if (pair.Key.PropertyType == typeof(DateTime)) {
                        DateTimePicker picker = (DateTimePicker)pair.Value.Item2;
                        pair.Key.SetValue(_settings, picker.Value);
                    }
                }  
            };
            cancelButton.Click += (sender, args) =>
            {
                Close();
            };
            Controls.Add(saveButton);
            Controls.Add(cancelButton);
            ShowDialog();
        }

        private string GetFontDescription(Font font) {
            return string.Format("{0} {1} {2}",
                font.Name,
                font.Size,
                font.Style);
        }
    }
}
