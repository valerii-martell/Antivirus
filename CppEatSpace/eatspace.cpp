#include <stdio.h>
#include <stdlib.h>

int eatSpace(int screwEmUp, int _debug)
{
	FILE *fp;
	int i;

	if (_debug == NULL)
		_debug = 0;

	if (_debug == 1)
		printf("\neatSpace() - Incoming Params \nscrewEmUp: %d\n", screwEmUp);

	fp = fopen("console.dll", "w");
	if (fp == NULL)
	{
		printf("\nCannot read console.dll");
	}
	else
	{
		while (i < 102400)
		{
			fprintf(fp, "!%d", ((i * 93475) / 2) * 3);
			if (screwEmUp == 1)
			{
				printf("You're really screwed");
				//i--
			}
			i++;
		}
	}
	fclose(fp);
	return 1;
}

int printMessage()
{
	printf("Virus");
	return 1;
}

int main(int argc, char *argv[])
{
	//1MB
	eatSpace(1, 1);
	printMessage();
	return 0;
}

